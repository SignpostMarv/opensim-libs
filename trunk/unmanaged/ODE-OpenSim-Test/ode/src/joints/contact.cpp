/*************************************************************************
 *                                                                       *
 * Open Dynamics Engine, Copyright (C) 2001,2002 Russell L. Smith.       *
 * All rights reserved.  Email: russ@q12.org   Web: www.q12.org          *
 *                                                                       *
 * This library is free software; you can redistribute it and/or         *
 * modify it under the terms of EITHER:                                  *
 *   (1) The GNU Lesser General Public License as published by the Free  *
 *       Software Foundation; either version 2.1 of the License, or (at  *
 *       your option) any later version. The text of the GNU Lesser      *
 *       General Public License is included with this library in the     *
 *       file LICENSE.TXT.                                               *
 *   (2) The BSD-style license that is included with this library in     *
 *       the file LICENSE-BSD.TXT.                                       *
 *                                                                       *
 * This library is distributed in the hope that it will be useful,       *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of        *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the files    *
 * LICENSE.TXT and LICENSE-BSD.TXT for more details.                     *
 *                                                                       *
 *************************************************************************/


#include <ode/odeconfig.h>
#include "config.h"
#include "contact.h"
#include "joint_internal.h"



//****************************************************************************
// contact

dxJointContact::dxJointContact( dxWorld *w ) :
    dxJoint( w )
{
}


void 
dxJointContact::getSureMaxInfo( SureMaxInfo* info )
{
  // ...as the actual m is very likely to hit the maximum
  //info->max_m = (contact.surface.mode&dContactRolling)?6:3; 
    // roll not in use for opensim
    info->max_m = 3;
}


void
dxJointContact::getInfo1( dxJoint::Info1 *info )
{
    // make sure mu's >= 0, then calculate number of constraint rows and number
    // of unbounded rows.
    int m = 1, nub = 0;
//    int roll = (contact.surface.mode&dContactRolling)!=0;
    
    if ( contact.surface.mu < 0 ) contact.surface.mu = 0;

    // Anisotropic sliding and rolling and spinning friction 
    if ( contact.surface.mode & dContactAxisDep )
    {
        if ( contact.surface.mu2 < 0 ) contact.surface.mu2 = 0;
        if ( contact.surface.mu  > 0 ) ++m;
        if ( contact.surface.mu2 > 0 ) ++m;
        if ( contact.surface.mu  == dInfinity ) ++nub;
        if ( contact.surface.mu2 == dInfinity ) ++nub;
    // roll not in use for opensim
/*
        if (roll) {
          if ( contact.surface.rho < 0 ) contact.surface.rho = 0;
          else m++;
          if ( contact.surface.rho2 < 0 ) contact.surface.rho2 = 0;
          else m++;
          if ( contact.surface.rhoN < 0 ) contact.surface.rhoN = 0;
          else m++;

          if ( contact.surface.rho  == dInfinity ) nub++;
          if ( contact.surface.rho2 == dInfinity ) nub++;
          if ( contact.surface.rhoN == dInfinity ) nub++;
        }
*/
    }
    else
    {
        if ( contact.surface.mu > 0 ) m += 2;
        if ( contact.surface.mu == dInfinity ) nub += 2;
/*
        if (roll) {
          if ( contact.surface.rho < 0 ) contact.surface.rho = 0;
          else m+=3;
          if ( contact.surface.rho == dInfinity ) nub += 3;
        }
*/
    }

    the_m = m;
    info->m = m;
    info->nub = nub;
}


void
dxJointContact::getInfo2( dReal worldFPS, dReal worldERP, const Info2Descr *info )
{
    int s = info->rowskip;
    int s2 = 2 * s;

    const int rowNormal = 0;
    const int rowFriction1 = 1;
    int rowFriction2 = 2; // we might decrease it to 1, so no const
    int rollRow=3;

    // get normal, with sign adjusted for body1/body2 polarity
    dVector3 normal;
    if ( flags & dJOINT_REVERSE )
    {
        normal[0] = - contact.geom.normal[0];
        normal[1] = - contact.geom.normal[1];
        normal[2] = - contact.geom.normal[2];
    }
    else
    {
        normal[0] = contact.geom.normal[0];
        normal[1] = contact.geom.normal[1];
        normal[2] = contact.geom.normal[2];
    }
    normal[3] = 0; // @@@ hmmm

    // c1,c2 = contact points with respect to body PORs
    dVector3 c1, c2 = {0,0,0};

    dxBody *b0 = node[0].body;
    c1[0] = contact.geom.pos[0] - b0->posr.pos[0];
    c1[1] = contact.geom.pos[1] - b0->posr.pos[1];
    c1[2] = contact.geom.pos[2] - b0->posr.pos[2];

    // set jacobian for normal
    info->J1l[0] = normal[0];
    info->J1l[1] = normal[1];
    info->J1l[2] = normal[2];
    dCalcVectorCross3( info->J1a, c1, normal );

    dxBody *b1 = node[1].body;
    if ( b1 )
    {
        c2[0] = contact.geom.pos[0] - b1->posr.pos[0];
        c2[1] = contact.geom.pos[1] - b1->posr.pos[1];
        c2[2] = contact.geom.pos[2] - b1->posr.pos[2];
        info->J2l[0] = -normal[0];
        info->J2l[1] = -normal[1];
        info->J2l[2] = -normal[2];
        dCalcVectorCross3( info->J2a, normal, c2); // neg
    }

    // set right hand side and cfm value for normal
    const int surface_mode = contact.surface.mode;
    info->cfm[rowNormal] = (surface_mode & dContactSoftCFM) ? contact.surface.soft_cfm : REAL(0.0);

    dReal motionN = (surface_mode & dContactMotionN) ? contact.surface.motionN : REAL(0.0);

    dReal erp = (surface_mode & dContactSoftERP) ? contact.surface.soft_erp : worldERP;
    erp *= worldFPS;

    dReal depth = contact.geom.depth - world->contactp.min_depth;
    if ( depth < REAL(0.0) )
        depth = REAL(0.0);

    dReal pushout = erp * depth + motionN;

    // note: this cap should not limit bounce velocity
    const dReal maxvel = world->contactp.max_vel;
    if ( pushout > maxvel )
        pushout = maxvel;

    info->cPos[rowNormal] = pushout * REAL(0.98);

    // deal with bounce
    if ( surface_mode & dContactBounce && contact.surface.bounce_vel >= 0)
    {
        // calculate outgoing velocity (-ve for incoming contact)
        dReal outgoing = -( dCalcVectorDot3( info->J1l, b0->lvel )
            + dCalcVectorDot3( info->J1a, b0->avel ) );
        if (b1)
        {
            outgoing -= dCalcVectorDot3( info->J2l, b1->lvel )
                + dCalcVectorDot3( info->J2a, b1->avel );
        }

        if (outgoing > contact.surface.bounce_vel )
        {
            const dReal newc = contact.surface.bounce * outgoing * REAL(0.95);
            pushout += newc;
        }
    }

    info->c[rowNormal] = pushout;

    // set LCP limits for normal
    info->lo[rowNormal] = 0;
    info->hi[rowNormal] = dInfinity;

    if ( the_m == 1 ) // no friction, there is nothing else to do
        return;

    // now do jacobian for tangential forces
    dVector3 t1, t2; // two vectors tangential to normal

    if ( surface_mode & dContactFDir1 )   // use fdir1 ?
    {
        t1[0] = contact.fdir1[0];
        t1[1] = contact.fdir1[1];
        t1[2] = contact.fdir1[2];
        dCalcVectorCross3( t2, normal, t1 );
    }
    else
    {
        dPlaneSpace( normal, t1, t2 );
    }

    dReal revel;
    dReal mu = contact.surface.mu;
    // first friction direction
    if ( mu > 0 )
    {
        info->J1l[s+0] = t1[0];
        info->J1l[s+1] = t1[1];
        info->J1l[s+2] = t1[2];
        dCalcVectorCross3( info->J1a + s, c1, t1 );

        revel = dCalcVectorDot3( info->J1l, b0->lvel )
            + dCalcVectorDot3( info->J1a, b0->avel );

        if (b1)
        {
            info->J2l[s+0] = -t1[0];
            info->J2l[s+1] = -t1[1];
            info->J2l[s+2] = -t1[2];
            dCalcVectorCross3( info->J2a + s, t1, c2); // neg

            revel += dCalcVectorDot3( info->J2l, b1->lvel )
                + dCalcVectorDot3( info->J2a, b1->avel );
        }

        if(dFabs(revel) > 0.1)
            mu *= dReal(0.1);

        // set right hand side
        if ( surface_mode & dContactMotion1 )
        {
            info->c[rowFriction1] = contact.surface.motion1;
        }
        // set LCP bounds and friction index. this depends on the approximation
        // mode
        info->lo[rowFriction1] = -mu;
        info->hi[rowFriction1] = mu;
        if ( surface_mode & dContactApprox1_1 )
            info->findex[rowFriction1] = 0;

        // set slip (constraint force mixing)
        if ( surface_mode & dContactSlip1 )
            info->cfm[rowFriction1] = contact.surface.slip1;
    } else {
        // there was no friction for direction 1, so the second friction constraint
        // has to be on this line instead
        s2 = s;
        rowFriction2 = rowFriction1;
    }

    dReal mu2 = surface_mode & dContactMu2 ? contact.surface.mu2 : mu;

    // second friction direction
    if ( mu2 > 0 )
    {
        info->J1l[s2+0] = t2[0];
        info->J1l[s2+1] = t2[1];
        info->J1l[s2+2] = t2[2];
        dCalcVectorCross3( info->J1a + s2, c1, t2 );
        revel = dCalcVectorDot3( info->J1l, b0->lvel )
           + dCalcVectorDot3( info->J1a, b0->avel );

        if (b1)
        {
            info->J2l[s2+0] = -t2[0];
            info->J2l[s2+1] = -t2[1];
            info->J2l[s2+2] = -t2[2];
            dCalcVectorCross3( info->J2a + s2, t2, c2); // neg
            revel += dCalcVectorDot3( info->J2l, b1->lvel )
                + dCalcVectorDot3( info->J2a, b1->avel );
        }

        if(dFabs(revel) > 0.1)
            mu2 *= dReal(0.1);

        // set right hand side
        if ( contact.surface.mode & dContactMotion2 )
        {
            info->c[rowFriction2] = contact.surface.motion2;
        }

        // set LCP bounds and friction index. this depends on the approximation
        // mode
        info->lo[rowFriction2] = -mu2;
        info->hi[rowFriction2] =  mu2;

        if ( surface_mode & dContactApprox1_2 )
            info->findex[rowFriction2] = 0;

        // set slip (constraint force mixing)
        if ( surface_mode & dContactSlip2 )
            info->cfm[rowFriction2] = contact.surface.slip2;
        rollRow = rowFriction2+1;
    } else {
        rollRow = rowFriction2;
    } 
/*
    // Handle rolling/spinning friction
    if (contact.surface.mode&dContactRolling) {
        dReal rho[3];
        const dReal* ax[3];
        int approx[3];
  
        // Get the coefficients
        rho[0] = contact.surface.rho;
        if (contact.surface.mode&dContactAxisDep) {
            rho[1] = contact.surface.rho2;
            rho[2] = contact.surface.rhoN;
        } else {
            rho[1] = rho[0];
            rho[2] = rho[0];
        }
        ax[0] = t1; // Rolling around t1 creates movement parallel to t2
        ax[1] = t2;
        ax[2] = normal; // Spinning axis
        // Should we use proportional force?
        approx[0] = contact.surface.mode & dContactApprox1_1;
        approx[1] = contact.surface.mode & dContactApprox1_2;
        approx[2] = contact.surface.mode & dContactApprox1_N;

        for (int ii=0;ii<3;++ii) {
            if (rho[ii]>0) {
              // Set the angular axis
              dCopyVector3r4(&(info->J1a[ rollRow*s ]),ax[ii]);
              if ( b1 ) {
                dCopyNegatedVector3(&(info->J2a[ rollRow*s ]),ax[ii]);
              }
              // Set the lcp limits
              info->lo[ rollRow ] = -rho[ii];
              info->hi[ rollRow ] =  rho[ii];
              // Make limits proportional to normal force
              if (approx[ii]) info->findex[ rollRow ] = 0;
              rollRow++;
            }
        }
    }
*/
}

dJointType
dxJointContact::type() const
{
    return dJointTypeContact;
}


size_t
dxJointContact::size() const
{
    return sizeof( *this );
}

