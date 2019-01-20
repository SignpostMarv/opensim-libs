using System;

namespace Warp3D
{
    /// <summary>
    /// Summary description for warp_Rasterizer.
    /// </summary>
    public class warp_Rasterizer
    {
        private bool materialLoaded = false;
        private bool lightmapLoaded = false;
        public bool ready = false;

        // Current material settings
        private int color = 0;
        private int currentColor = 0;
        private int reflectivity = 0;
        private warp_Texture texture = null;
        private int[] tpixels = null;
        private int[] envmap = null;
        private int[] diffuse = null;
        private int[] specular = null;
        private int tw = 0;
        private int th = 0;
        private int tbitW = 0;
        private int tbitH = 0;

        // Rasterizer hints
        private int mode = 0;
        private const int F = 0;      // FLAT
        private const int W = 1;  // WIREFRAME
        private const int P = 2;      // PHONG
        private const int E = 4;      // ENVMAP
        private const int T = 8;  // TEXTURED
        private int SHADED = 0;

        warp_Vertex p1, p2, p3, tempVertex;

        private int
        bkgrd, c, s, lutID, //lutID is position in LUT (diffuse,envmap,specular)
        x1, x2, x3, x4, y1, y2, y3, z1, z2, z3, z4,
        x, y, z, dx, dy, dz, offset, pos, temp,
        xL, xR, xBase, zBase, xMax, dxL, dxR, dzBase,

        nx1, nx2, nx3, nx4, ny1, ny2, ny3, ny4,
        nxBase, nyBase,
        dnx, dny, nx, ny,
        dnxBase, dnyBase;

        private float
        tx1, tx2, tx3, tx4, ty1, ty2, ty3, ty4,
        txBase, tyBase,
        tx, ty, sw,
        dtxBase, dtyBase,
        dtx, dty,
        sw1, sw2, sw3, sw4, swBase, dsw, dswBase;

        warp_Screen screen;
        int[] zBuffer;
        int width, height;

        // Constructor
        public warp_Rasterizer(warp_RenderPipeline pipeline)
        {
            SHADED = P | E | T;

            rebuildReferences(pipeline);
            loadLightmap(pipeline.lightmap);
        }

        // References
        public void rebuildReferences(warp_RenderPipeline pipeline)
        {
            screen = pipeline.screen;
            zBuffer = pipeline.zBuffer;
            width = screen.width;
            height = screen.height;
        }

        public void clean()
        {
            screen = null;
            zBuffer = null;
        }

        // Lightmap loader
        public void loadLightmap(warp_Lightmap lm)
        {
            if (lm == null)
                return;
            diffuse = lm.diffuse;
            specular = lm.specular;
            lightmapLoaded = true;
            ready = lightmapLoaded && materialLoaded;
        }

        // Material loader
        public void loadMaterial(warp_Object obj)
        {
            warp_Material material = obj.material;
            color = material.color;
            reflectivity = material.reflectivity;
            texture = material.texture;
            if (material.envmap != null)
                envmap = material.envmap.pixel;
            else
                envmap = null;

            if (texture != null)
            {
                if (obj.projectedmaxMips < 2)
                {
                    color = warp_Color.multiply(color, texture.averageColor);
                    texture = null;
                }
                else
                {
                    int mip = obj.projectedmaxMips - 1;
                    if (mip > texture.maxmips)
                        mip = texture.maxmips;

                    if (texture.mips[mip] != null)
                    {
                        tpixels = texture.mips[mip];
                        tbitW = texture.mipsBitWidth[mip];
                        tbitH = texture.mipsBitHeight[mip];
                        tw = texture.mipstw[mip] - 1;
                        th = texture.mipsth[mip] - 1;
                    }
                    else
                    {
                        tpixels = texture.pixel;
                        tw = texture.width - 1;
                        th = texture.height - 1;
                        tbitW = texture.bitWidth;
                        tbitH = texture.bitHeight;
                    }
                }
            }

            mode = 0;
            if (!material.flat)
                mode |= P;
            if (envmap != null)
                mode |= E;
            if (texture != null)
                mode |= T;
            if (material.wireframe)
                mode |= W;
            materialLoaded = true;
            ready = lightmapLoaded && materialLoaded;
        }

        public void loadFastMaterial(warp_Object obj)
        {
            color = obj.fastcolor;
            reflectivity = obj.fastreflectivity;
            texture = obj.fasttexture;
            envmap = obj.fastenvmappixels;

            if (texture != null)
            {
                tpixels = obj.fasttpixels;
                tw = obj.fasttw;
                th = obj.fastth;
                tbitW = obj.fasttbitw;
                tbitH = obj.fasttbith;
            }

            mode = obj.fastmode;
            materialLoaded = true;
            ready = lightmapLoaded && materialLoaded;
        }

        public void render(warp_Triangle tri)
        {
            if (!ready)
            {
                return;
            }
            if (tri.parent == null)
            {
                return;
            }
            if ((mode & W) != 0)
            {
                drawWireframe(tri, color);
                if ((mode & W) == 0)
                {
                    return;
                }
            }

            p1 = tri.p1;
            p2 = tri.p2;
            p3 = tri.p3;

            if (p1.y > p2.y)
            {
                tempVertex = p1;
                p1 = p2;
                p2 = tempVertex;
            }
            if (p2.y > p3.y)
            {
                tempVertex = p2;
                p2 = p3;
                p3 = tempVertex;
            }
            if (p1.y > p2.y)
            {
                tempVertex = p1;
                p1 = p2;
                p2 = tempVertex;
            }

            if (p1.y >= height)
                return;

            if (p3.y < 0)
                return;

            if (p1.y == p3.y)
                return;

            if (mode == F)
            {
                lutID = (int)(tri.n2.x * 127 + 127) + ((int)(tri.n2.y * 127 + 127) << 8);
                c = warp_Color.multiply(color, diffuse[lutID]);
                s = warp_Color.scale(specular[lutID], reflectivity);
                currentColor = warp_Color.add(c, s);
            }

            x1 = p1.x << 8;
            x2 = p2.x << 8;
            x3 = p3.x << 8;
            y1 = p1.y;
            y2 = p2.y;
            y3 = p3.y;

            int dy21 = y2 - y1;
            int dy31 = y3 - y1;

            x4 = x1 + (x3 - x1) * dy21 / dy31;

            dx = (x4 - x2) >> 8;
            if (dx == 0)
                return;

            x1 <<= 8;
            x2 <<= 8;
            x3 <<= 8;
            x4 <<= 8;

            temp = (dy21 << 8) / dy31;

            z1 = p1.z;
            z3 = p3.z;
            z4 = z1 + ((z3 - z1) >> 8) * temp;
            z2 = p2.z;
            dz = (z4 - z2) / dx;

            nx1 = p1.nx;
            nx3 = p3.nx;
            nx4 = nx1 + ((nx3 - nx1) >> 8) * temp;
            nx2 = p2.nx;
            dnx = (nx4 - nx2) / dx;

            ny1 = p1.ny;
            ny3 = p3.ny;
            ny4 = ny1 + ((ny3 - ny1) >> 8) * temp;
            ny2 = p2.ny;
            dny = (ny4 - ny2) / dx;

            float tf = (float)dy21 / (float)dy31;

            tx1 = p1.tx * tw;
            tx3 = p3.tx * tw;
            tx4 = tx1 + ((tx3 - tx1) * tf);
            tx2 = p2.tx * tw;
            dtx = (tx4 - tx2) / dx;

            ty1 = p1.ty * th;
            ty3 = p3.ty * th;
            ty4 = ty1 + ((ty3 - ty1) * tf);
            ty2 = p2.ty * th;
            dty = (ty4 - ty2) / dx;

            sw1 = p1.invZ;
            sw3 = p3.invZ;
            sw4 = sw1 + ((sw3 - sw1) * tf);
            sw2 = p2.invZ;
            dsw = (sw4 - sw2) / dx;

            if (dx < 0)
            {
                temp = x2;
                x2 = x4;
                x4 = temp;
                z2 = z4;
                tx2 = tx4;
                ty2 = ty4;
                sw2 = sw4;
                nx2 = nx4;
                ny2 = ny4;
            }


            dy = dy21;
            if (y2 >= 0 && dy > 0)
            {
                dxL = (x2 - x1) / dy;
                dxR = (x4 - x1) / dy;
                dzBase = (z2 - z1) / dy;
                dnxBase = (nx2 - nx1) / dy;
                dnyBase = (ny2 - ny1) / dy;
                dtxBase = (tx2 - tx1) / dy;
                dtyBase = (ty2 - ty1) / dy;
                dswBase = (sw2 - sw1) / dy;

                xBase = x1;
                xMax = x1;
                zBase = z1;
                nxBase = nx1;
                nyBase = ny1;
                txBase = tx1;
                tyBase = ty1;
                swBase = sw1;

                if (y1 < 0)
                {
                    xBase -= y1 * dxL;
                    xMax -= y1 * dxR;
                    zBase -= y1 * dzBase;
                    nxBase -= y1 * dnxBase;
                    nyBase -= y1 * dnyBase;
                    txBase -= y1 * dtxBase;
                    tyBase -= y1 * dtyBase;
                    swBase -= y1 * dswBase;
                    y1 = 0;
                }

                if (y2 > height)
                    y2 = height;
                offset = y1 * width;
                for (y = y1; y < y2; y++)
                {
                    renderLine();
                }
            }

            dy = y3 - y2;
            if (y2 < height && dy > 0)
            {
                dxL = (x3 - x2) / dy;
                dxR = (x3 - x4) / dy;
                dzBase = (z3 - z2) / dy;
                dnxBase = (nx3 - nx2) / dy;
                dnyBase = (ny3 - ny2) / dy;
                dtxBase = (tx3 - tx2) / dy;
                dtyBase = (ty3 - ty2) / dy;
                dswBase = (sw3 - sw2) / dy;

                xBase = x2;
                xMax = x4;
                zBase = z2;
                nxBase = nx2;
                nyBase = ny2;
                txBase = tx2;
                tyBase = ty2;
                swBase = sw2;

                if (y2 < 0)
                {
                    xBase -= y2 * dxL;
                    xMax -= y2 * dxR;
                    zBase -= y2 * dzBase;
                    nxBase -= y2 * dnxBase;
                    nyBase -= y2 * dnyBase;
                    txBase -= y2 * dtxBase;
                    tyBase -= y2 * dtyBase;
                    swBase -= y2 * dswBase;
                    y2 = 0;
                }

                if (y3 > height)
                    y3 = height;
                offset = y2 * width;

                for (y = y2; y < y3; y++)
                {
                    renderLine();
                }
            }
        }

        private void renderLine()
        {
            xL = xBase >> 16;
            xR = xMax >> 16;
            z = zBase;
            nx = nxBase;
            ny = nyBase;
            tx = txBase;
            ty = tyBase;
            sw = swBase;

            if (xL < 0)
            {
                z -= xL * dz;
                nx -= xL * dnx;
                ny -= xL * dny;
                tx -= xL * dtx;
                ty -= xL * dty;
                sw -= xL * dsw;
                xL = 0;
            }
            if (xR > width)
                xR = width;

            if (mode == F)
                renderLineF();
            else if ((mode & SHADED) == P)
                renderLineP();
            else if ((mode & SHADED) == E)
                renderLineE();
            else if ((mode & SHADED) == T)
                renderLineT();
            else if ((mode & SHADED) == (P | E))
                renderLinePE();
            else if ((mode & SHADED) == (P | T))
                renderLinePT();
            else if ((mode & SHADED) == (P | E | T))
                renderLinePET();

            offset += width;
            xBase += dxL;
            xMax += dxR;
            zBase += dzBase;
            nxBase += dnxBase;
            nyBase += dnyBase;
            txBase += dtxBase;
            tyBase += dtyBase;
            swBase += dswBase;
        }

        // Fast scanline rendering
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private unsafe void renderLineF()
        {
            fixed (int* sp = screen.pixels, zB = zBuffer)
            {
                for (x = xL; x < xR; x++)
                {
                    pos = x + offset;
                    if (z < zBuffer[pos])
                    {
                        bkgrd = sp[pos];
                        c = warp_Color.overSolid(bkgrd, currentColor);
                        sp[pos] = c;
                        zB[pos] = z;
                    }
                    z += dz;
                }
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private unsafe void renderLineP()
        {
            fixed (int* sp = screen.pixels, zB = zBuffer, df = diffuse, spc = specular)
            {
                for (x = xL; x < xR; x++)
                {
                    pos = x + offset;
                    if (z < zB[pos])
                    {
                        lutID = ((nx >> 16) & 255) + (((ny >> 16) & 255) << 8);
                        bkgrd = sp[pos];
                        c = warp_Color.multiply(color, df[lutID]);
                        s = spc[lutID];
                        s = warp_Color.scale(s, reflectivity);
                        c = warp_Color.overSolid(bkgrd, c);
                        c = warp_Color.add(c, s);
                        sp[pos] = c;
                        zB[pos] = z;
                    }
                    z += dz;
                    nx += dnx;
                    ny += dny;
                }
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private unsafe void renderLineE()
        {
            fixed (int* sp = screen.pixels, zB = zBuffer, spc = specular, env = envmap)
            {
                for (x = xL; x < xR; x++)
                {
                    pos = x + offset;
                    if (z < zB[pos])
                    {
                        lutID = ((nx >> 16) & 255) + (((ny >> 16) & 255) << 8);
                        bkgrd = sp[pos];
                        s = warp_Color.add(spc[lutID], env[lutID]);
                        s = warp_Color.scale(s, reflectivity);
                        c = warp_Color.overSolid(bkgrd, s);
                        sp[pos] = c;
                        zB[pos] = z;
                    }
                    z += dz;
                    nx += dnx;
                    ny += dny;
                }
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private unsafe void renderLineT()
        {
            fixed (int* sp = screen.pixels, zB = zBuffer, tp = tpixels)
            {
                for (x = xL; x < xR; x++)
                {
                    pos = x + offset;
                    if (z < zB[pos])
                    {
                        bkgrd = sp[pos];
                        c = tp[(((int)(tx / sw)) & tw) + ((((int)(ty / sw)) & th) << tbitW)];
                        c = warp_Color.multiply(color, c);
                        c = warp_Color.overSolid(bkgrd, c);
                        sp[pos] = c;
                        zB[pos] = z;
                    }
                    z += dz;
                    tx += dtx;
                    ty += dty;
                    sw += dsw;
                }
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private unsafe void renderLinePE()
        {
            fixed (int* sp = screen.pixels, zB = zBuffer, df = diffuse, spc = specular, env = envmap)
            {
                for (x = xL; x < xR; x++)
                {
                    pos = x + offset;
                    if (z < zB[pos])
                    {
                        lutID = ((nx >> 16) & 255) + (((ny >> 16) & 255) << 8);

                        bkgrd = sp[pos];
                        c = warp_Color.multiply(color, df[lutID]);
                        s = warp_Color.add(spc[lutID], env[lutID]);
                        s = warp_Color.scale(s, reflectivity);
                        c = warp_Color.overSolid(bkgrd, c);
                        c = warp_Color.add(c, s);
                        sp[pos] = c;
                        zB[pos] = z;
                    }
                    z += dz;
                    nx += dnx;
                    ny += dny;
                }
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private unsafe void renderLinePT()
        {
            fixed (int* sp = screen.pixels, zB = zBuffer, df = diffuse, spc = specular, tp = tpixels)
            {
                for (x = xL; x < xR; x++)
                {
                    pos = x + offset;
                    if (z < zB[pos])
                    {
                        lutID = ((nx >> 16) & 255) + (((ny >> 16) & 255) << 8);

                        bkgrd = sp[pos];
                        c = tp[(((int)(tx / sw)) & tw) + ((((int)(ty / sw)) & th) << tbitW)];
                        c = warp_Color.multiply(color, c);
                        c = warp_Color.multiply(c, df[lutID]);
                        s = warp_Color.scale(spc[lutID], reflectivity);
                        c = warp_Color.overSolid(bkgrd, c);
                        c = warp_Color.add(c, s);
                        sp[pos] = c;
                        zB[pos] = z;
                    }

                    z += dz;
                    nx += dnx;
                    ny += dny;
                    tx += dtx;
                    ty += dty;
                    sw += dsw;
                }
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private unsafe void renderLinePET()
        {
            fixed (int* sp = screen.pixels, zB = zBuffer, df = diffuse, spc = specular, tp = tpixels, env = envmap)
            {
                for (x = xL; x < xR; x++)
                {
                    pos = x + offset;
                    if (z < zB[pos])
                    {
                        lutID = ((nx >> 16) & 255) + (((ny >> 16) & 255) << 8);
                        bkgrd = sp[pos];
                        c = tp[(((int)(tx / sw)) & tw) + ((((int)(ty / sw)) & th) << tbitW)];
                        c = warp_Color.multiply(color, c);
                        c = warp_Color.multiply(c, df[lutID]);
                        s = warp_Color.add(spc[lutID], env[lutID]);
                        s = warp_Color.scale(s, reflectivity);
                        c = warp_Color.overSolid(bkgrd, c);
                        c = warp_Color.add(c, s);

                        sp[pos] = c;
                        zB[pos] = z;
                    }
                    z += dz;
                    nx += dnx;
                    ny += dny;
                    tx += dtx;
                    ty += dty;
                    sw += dsw;
                }
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private void drawWireframe(warp_Triangle tri, int defaultcolor)
        {
            drawLine(tri.p1, tri.p2, defaultcolor);
            drawLine(tri.p2, tri.p3, defaultcolor);
            drawLine(tri.p3, tri.p1, defaultcolor);
        }

        private unsafe void drawLine(warp_Vertex a, warp_Vertex b, int color)
        {
            warp_Vertex temp;
            if ((a.clipcode & b.clipcode) != 0)
                return;

            dx = (int)Math.Abs(a.x - b.x);
            dy = (int)Math.Abs(a.y - b.y);
            dz = 0;

            fixed (int* sp = screen.pixels, zB = zBuffer)
            {
                if (dx > dy)
                {
                    if (a.x > b.x)
                    {
                        temp = a;
                        a = b;
                        b = temp;
                    }
                    if (dx > 0)
                    {
                        dz = (b.z - a.z) / dx;
                        dy = ((b.y - a.y) << 16) / dx;
                    }
                    z = a.z;
                    y = a.y << 16;
                    for (x = a.x; x <= b.x; x++)
                    {
                        y2 = y >> 16;
                        if (warp_Math.inrange(x, 0, width - 1) && warp_Math.inrange(y2, 0, height - 1))
                        {
                            offset = y2 * width;
                            if (z < zB[x + offset])
                            {
                                {
                                    sp[x + offset] = color;
                                    zB[x + offset] = z;
                                }
                            }
                        }
                        z += dz;
                        y += dy;
                    }
                }
                else
                {
                    if (a.y > b.y)
                    {
                        temp = a;
                        a = b;
                        b = temp;
                    }
                    if (dy > 0)
                    {
                        dz = (b.z - a.z) / dy;
                        dx = ((b.x - a.x) << 16) / dy;
                    }
                    z = a.z;
                    x = a.x << 16;
                    for (y = a.y; y <= b.y; y++)
                    {
                        x2 = x >> 16;
                        if (warp_Math.inrange(x2, 0, width - 1) && warp_Math.inrange(y, 0, height - 1))
                        {
                            offset = y * width;
                            if (z < zB[x2 + offset])
                            {
                                {
                                    sp[x2 + offset] = color;
                                    zB[x2 + offset] = z;
                                }
                            }
                        }
                        z += dz;
                        x += dx;
                    }
                }
            }
        }
    }
}
