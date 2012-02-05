using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using MCI;

using Ode;


namespace TestWarp
{

    public partial class Warp : Form
	{
        /*
         * ODE stuff
         */
		protected Ode.World _world= new Ode.World();
		protected Ode.Space _space = new Ode.HashSpace();
		protected Ode.JointGroup _contactGroup = new Ode.JointGroup();
		private Ode.CollideHandler collideCallback;		
		protected Ode.Geom _floor;
        protected PlaneGeom[] _walls;
        private const int HALF_WIDTH = 256;
        private const int HALF_LENGTH = 256;
        protected bool _useStepFast = false;
        private const float MAX_STEP_SIZE = 0.1f;
        private DateTime lastframe = DateTime.MinValue;
        private double leftOverTime = 0;
        protected bool pauseSim;
        Ode.Vector3 _prevpos = new Vector3();
		int _bodyIndex = 0;
        Body[] _body = new Body[ 500 ];
        Geom _geom;
		

        /*
         * End
         */


        private Media media = new Media();

        Timer Clock;
        enum State
        {
            demo1, demo2, demo3, demo4, demo5, demo6
        }


        State _state = State.demo1;
  

		public Warp()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
            warp3D1.DisplayDefaultScene();
            //media.PlayWavResource( "tunes.Bass_Is_Extra_90bpm.wav" );

            Clock = new Timer();
            Clock.Interval = 50;
          
            Clock.Tick += new EventHandler( Timer_Tick );

			Clock.Start();
		}

        public void Timer_Tick( object sender, EventArgs eArgs )
        {
            if ( sender == Clock )
            {
                switch (_state)
                {
                    case State.demo1:
                        warp3D1.RotateObject( "LensFlare1", -.05f, -.15f, -.05f );
                        warp3D1.RotateScene( .05f, .05f, .05f );
                        break;

                    case State.demo6:
                        UpdateWorld();

						for ( int q = 0; q < _body.Length; q++ )
						{
							if ( _body[ q ] != null )
							{
								Ode.Vector3 pos = _body[ q ].Position;

								warp3D1.RotateSelf( "cube"+q, OdeWarpHelpers.Ode2WarpQuaternion( _body[ q ].Quaternion ) );
								warp3D1.SetPos( "cube" + q, pos.X, pos.Y, pos.Z );
							}
						}
                        
                        break;

                    case State.demo4:
                        warp3D1.RotateModelSelf( "satellite", -.15f, -.15f, -.15f );
                        warp3D1.RotateModel( "satellite", -.02f, -.02f, -.02f );
                        break;

                    case State.demo3:
                    case State.demo2:
                        warp3D1.RotateScene( .05f, .05f, .05f );
                        break;
                }
                
                warp3D1.Render();
                warp3D1.Refresh();
            }
        }

		private void Load_Scene_Click(object sender, EventArgs e)
		{
            _state = State.demo1;

			warp3D1.CreateScene(512, 512);
            warp3D1.SetBackgroundColor( 0xECE9D8 );

            Hashtable objects = warp3D1.Import3Ds( "chair", "DesignerChair3.3ds", true );

			warp3D1.AddLight("Light1", -1f, -1f, -1f, 0xffffff, 320, 200);
			warp3D1.AddLight("Light2", .2f, .2f, 1f, 0xffffff, 320, 200);

			warp3D1.AddMaterial("seat");
			warp3D1.SetTexture("seat", "glass.jpg");
			warp3D1.SetReflectivity("seat", 80);
			warp3D1.SetTransparency("seat", 180);
            warp3D1.SetWireframe( "seat", true );

			warp3D1.AddMaterial("wood");
			warp3D1.SetTexture("wood", "cream.jpg");
			warp3D1.SetReflectivity("wood", 100);

			warp3D1.AddMaterial("strut");
			warp3D1.SetTexture("strut", "chrome.jpg");
			warp3D1.SetReflectivity("strut", 255);
			warp3D1.SetEnvMap("strut", "skymap.jpg");

			warp3D1.SetObjectMaterial("chair_Object", "seat");
            warp3D1.SetObjectMaterial( "chair_Object", "seat" );
            warp3D1.SetObjectMaterial( "chair_Object01", "wood" );
            warp3D1.SetObjectMaterial( "chair_Object02", "wood" );
            warp3D1.SetObjectMaterial( "chair_Object03", "strut" );
            warp3D1.SetObjectMaterial( "chair_Object04", "strut" );
            warp3D1.SetObjectMaterial( "chair_Object05", "strut" );
            warp3D1.SetObjectMaterial( "chair_Object06", "strut" );
            warp3D1.SetObjectMaterial( "chair_Object07", "strut" );
            warp3D1.SetObjectMaterial( "chair_Object08", "strut" );
            warp3D1.SetObjectMaterial( "chair_Object09", "strut" );

			warp3D1.NormaliseScene();

			warp3D1.Render();
			warp3D1.Refresh();
		}

        private void button_Flare1_Click( object sender, EventArgs e )
        {
            _state = State.demo2;

            warp3D1.CreateScene( 512, 512 );

            warp3D1.SetBackgroundMaterial( "stardust.material" );
            warp3D1.AddMaterial( "metal" );
            warp3D1.SetEnvMap( "metal", "chrome.jpg" );
            warp3D1.SetReflectivity( "metal",  255 );

            warp3D1.AddLight( "Light1", 0.2f, 0.2f, 1f, 0xffffff, 320, 80 );

            Hashtable objects = warp3D1.Import3Ds( "wobble", "wobble.3ds", true );
            foreach ( DictionaryEntry myDE in objects )
            {
                string name = (string)myDE.Key;
                warp3D1.SetObjectMaterial( name, "metal" );
            }

            warp3D1.AddLensFlare("LensFlare1");
            warp3D1.NormaliseScene();

            warp3D1.ScaleObject( "LensFlare1", 60f );
            warp3D1.ScaleScene( 0.5f, 0.5f, 0.5f );

            warp3D1.Render();
            warp3D1.Refresh();
        }

        private void button_Stones_Click( object sender, EventArgs e )
        {
            _state = State.demo3;

            warp3D1.CreateScene( 512, 512 );
            warp3D1.SetBackgroundColor( 0xECE9D8 );

            warp3D1.AddMaterial( "Stone1" );
            warp3D1.SetTexture( "Stone1", "stone1.jpg" );

            warp3D1.AddMaterial( "Stone2" );
            warp3D1.SetTexture( "Stone2", "stone2.jpg" );
            
            warp3D1.AddMaterial( "Stone3" );
            warp3D1.SetTexture( "Stone3", "stone3.jpg" );
            
            warp3D1.AddMaterial( "Stone4" );
            warp3D1.SetTexture( "Stone4", "stone4.jpg" );

            warp3D1.AddLight( "Light1", 0.2f, 0.2f, 1f , 0xFFFFFF, 144, 120  );
            warp3D1.AddLight( "Light2", -1f, -1f, 1f , 0x332211, 100, 40  );
            warp3D1.AddLight( "Light3", -1f, -1f, 1f , 0x666666, 200, 120  );

            Hashtable objects = warp3D1.Import3Ds( "wobble", "wobble.3ds", true );
            foreach ( DictionaryEntry myDE in objects )
            {
                string name = ( string )myDE.Key;
                warp3D1.ProjectFrontal( name );
            }

            warp3D1.SetObjectMaterial( "wobble_Sphere1", "Stone1" );
            warp3D1.SetObjectMaterial( "wobble_Wobble1", "Stone2" );
            warp3D1.SetObjectMaterial( "wobble_Wobble2", "Stone3" );
            warp3D1.SetObjectMaterial( "wobble_Wobble3", "Stone4" );

            warp3D1.NormaliseScene();
            warp3D1.ShiftObject( "Wobble3", 55f, 1f, 1f );
            warp3D1.ScaleScene( 0.5f, 0.5f, 0.5f );

            warp3D1.Render();
            warp3D1.Refresh();
        }

        private void button_Bounce_Click( object sender, EventArgs e )
        {
            _state = State.demo4;

            warp3D1.CreateScene( 512, 512 );
            warp3D1.SetBackgroundMaterial( "stardust.material" );

            warp3D1.AddMaterial( "earth" );
            warp3D1.SetTexture( "earth", "land_shallow_topo_1024.jpg" );
            warp3D1.SetReflectivity( "earth", 200 );

            warp3D1.AddMaterial( "panel" );
            warp3D1.SetTexture( "panel", "icesolar.JPG" );
            warp3D1.SetReflectivity( "panel", 225 );

            warp3D1.AddLight( "Light1", 0.2f, 0.2f, 1f, 0xFFFFFF, 144, 120 );
            warp3D1.AddLight( "Light2", -1f, -1f, 1f, 0x332211, 100, 40 );
            warp3D1.AddLight( "Light3", -1f, -1f, 1f, 0x666666, 200, 120 );

            Hashtable objects = warp3D1.Import3Ds( "satellite", "voyager.3ds", true );
            foreach ( DictionaryEntry myDE in objects ) 
            {
                string name = ( string )myDE.Key;
                warp3D1.SetObjectMaterial( name, "panel" );
            }

            warp3D1.ScaleModel( "satellite", .002f );
            warp3D1.TranslateModel( "satellite", .66f,.66f,0f );

            warp3D1.AddSphere( "Sphere1", .65f, 32 );
            warp3D1.SetObjectMaterial( "Sphere1", "earth" );

            warp3D1.Render();
            warp3D1.Refresh();
            Clock.Start();
        }

        private void button_Studio_Click( object sender, EventArgs e )
        {
            _state = State.demo5;

            warp3D1.CreateScene( 512, 512 );
            warp3D1.SetBackgroundColor( 0xECE9D8 );

            warp3D1.AddMaterial( "panel" );
            warp3D1.SetTexture( "panel", "voyager.jpg" );
            warp3D1.SetReflectivity( "panel", 225 );

            warp3D1.AddMaterial( "default" );
            warp3D1.SetReflectivity( "default", 225 );
            warp3D1.SetTexture( "default", "chrome.jpg" );

            warp3D1.AddLight( "Light1", 0f, 0f, 1f, 0xFFFFFF, 320, 120 );

            Hashtable objects = warp3D1.Import3Ds( "voyager", "voyager.3ds", true );
            foreach ( DictionaryEntry myDE in objects )
            {
                string name = ( string )myDE.Key;
                warp3D1.SetObjectMaterial( name, "default" );
            }

            warp3D1.SetObjectMaterial( "voyager_Plaque", "panel" );
            warp3D1.NormaliseScene();
           
        }

        private void button_Physics_Click( object sender, EventArgs e )
        {

            _state = State.demo6;

            warp3D1.CreateScene( 512, 512 );
            warp3D1.SetBackgroundColor( 0xECE9D8 );

            warp3D1.AddMaterial( "default", 0x00dd00 );
            warp3D1.SetReflectivity( "default", 255 );

			warp3D1.AddMaterial( "yellow", 0xffff00 );
			warp3D1.SetReflectivity( "yellow", 200 );

			warp3D1.AddMaterial( "floor", 0xcccccc );
            warp3D1.AddPlane("plane1", 10f);
            warp3D1.SetObjectMaterial("plane1", "floor");


            warp3D1.AddLight( "Light1", 0.2f, 0.2f, 1f, 0xFFFFFF, 144, 120 );
            warp3D1.AddLight( "Light2", -1f, 0f, 0f, 0x332211, 100, 40 );
            warp3D1.AddLight( "Light3", 1f, 0f, 0f, 0x666666, 200, 120 );
            warp3D1.AddCube( "cube0", 2f );
			warp3D1.SetObjectMaterial( "cube0", "default" );
            
            _world.SetGravity( 0, -9.81f, 0 );
            _world.CFM = 1e-6f;
            _floor = new Ode.PlaneGeom( _space, 0, 1, 0, 0 );
            collideCallback = new Ode.CollideHandler( this.CollideCallback );

            _walls = new PlaneGeom[ 4 ];
            _walls[ 0 ] = new PlaneGeom( _space, 1, 0, 0, -10 );
            _walls[ 1 ] = new PlaneGeom( _space, -1, 0, 0, -10 );
            _walls[ 2 ] = new PlaneGeom( _space, 0, 0, 1, -10 );
            _walls[ 3 ] = new PlaneGeom( _space, 0, 0, -1, -10 );

            InitialiseWorld();

            warp3D1.ShiftDefaultCamera( 0f, 0f, -20f );

            button_Body.Enabled = true;
            button_Sphere.Enabled = true;
        }

        protected virtual void CollideCallback( Ode.Geom o1, Ode.Geom o2 )
        {
            ContactGeom[] cgeoms = o1.Collide( o2, 30 );
            if ( cgeoms.Length > 0 )
            {
                Contact[] contacts = Contact.FromContactGeomArray( cgeoms );

                for ( int i = 0; i < contacts.Length; i++ )
                {
                    contacts[ i ].Surface.mode = SurfaceMode.Bounce;
                    contacts[ i ].Surface.mu = 100;  //World.Infinity;
                    contacts[ i ].Surface.bounce = 0.70f;
                    contacts[ i ].Surface.bounce_vel = 0.05f;
                    ContactJoint cj = new ContactJoint( _world, _contactGroup, contacts[ i ] );
                    cj.Attach( contacts[ i ].Geom.Geom1.Body, contacts[ i ].Geom.Geom2.Body );
                }
            }
        }

        private void UpdateWorld()
        {
            if ( lastframe == DateTime.MinValue || pauseSim )
            {
                lastframe = DateTime.Now;
                return;
            }

            DateTime thisframe = DateTime.Now;
            TimeSpan elapsed = thisframe - lastframe;

            float t = ( float )( elapsed.TotalSeconds + leftOverTime );
            float st;

            if ( t == 0 )
                return;

            while ( t > 0.02 )
            {
                if ( t > MAX_STEP_SIZE )
                {
                    st = MAX_STEP_SIZE;
                    t -= MAX_STEP_SIZE;
                }
                else
                {
                    st = t;
                    t = 0;
                }

                _contactGroup.Empty();
                UpdateWorldForces( st );

                _space.Collide( collideCallback );

                if ( _useStepFast )
                    _world.StepFast1( st, 5 );
                else
                    _world.Step( st );
            }
            leftOverTime = t;

            lastframe = thisframe;
        }

        protected void InitialiseWorld()
        {
            Mass mass;
                mass = new Mass();
            mass.SetBox( 500.0f, 2f, 2f, 2f );
         
            _body[_bodyIndex] = new Body( _world );
            _body[_bodyIndex].Position = new Ode.Vector3( 0, 8, 0 );
			_body[ _bodyIndex ].SetMass( mass );           
			_body[_bodyIndex].Geoms.Add( new BoxGeom( _space, 2f, 2f, 2f ) );
        }

        protected void RenderWorld()
        {
        }

        protected void UpdateWorldForces( double stepSize )
        {
            // eg _body[n].AddForce( new Ode.Vector3( 0, 0, 1.0f ) );
        }

		private void button_Body_Click( object sender, EventArgs e )
		{
            if ( _state != State.demo6 )
            {
                return;
            }

			_bodyIndex++;

			warp3D1.AddCube( "cube" + _bodyIndex, 2f );
			warp3D1.SetObjectMaterial( "cube" + _bodyIndex, "default" );


			_body[ _bodyIndex ] = new Body( _world );
			_body[ _bodyIndex ].Position = new Ode.Vector3( 0, 8, 0 );
			_body[ _bodyIndex ].Geoms.Add( new BoxGeom( _space, 2f, 2f, 2f ) );

		}

		private void button_Sphere_Click( object sender, EventArgs e )
		{
            if ( _state != State.demo6 )
            {
                return;
            }

            _bodyIndex++;

			warp3D1.AddSphere( "cube" + _bodyIndex, 1f, 24 );
			warp3D1.SetObjectMaterial( "cube" + _bodyIndex, "yellow" );

			_body[ _bodyIndex ] = new Body( _world );
			_body[ _bodyIndex ].Position = new Ode.Vector3( 0, 8, 0 );
			_body[ _bodyIndex ].Geoms.Add( new SphereGeom(_space, 1f));
		}

        private void button_Cancel_Click( object sender, EventArgs e )
        {

        }

	}

}