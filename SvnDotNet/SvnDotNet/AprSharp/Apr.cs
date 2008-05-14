//  AprSharp, a wrapper library around the Apache Portable Runtime Library
#region Copyright (C) 2004 SOFTEC sa.
//
//  This library is free software; you can redistribute it and/or
//  modify it under the terms of the GNU Lesser General Public
//  License as published by the Free Software Foundation; either
//  version 2.1 of the License, or (at your option) any later version.
//
//  This library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
//  Sources, support options and lastest version of the complete library
//  is available from:
//		http://www.softec.st/AprSharp
//		Support@softec.st
//
//  Initial authors : 
//		Denis Gervalle
//		Olivier Desaive
#endregion
//
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;

namespace PumaCode.SvnDotNet.AprSharp {

    ///<summary>Embeds all APR external calls</summary>
    ///<remark>Take care to call apr_initialize</remark>
    public sealed class Apr {
        // TODO: figure out if it's possible to dynamically determine library name @ runtime
#if WIN32
        private const string AprLibrary = "libapr";
#else
		private const string AprLibrary = "apr-1";		
#endif

        // no instance constructor !
        private Apr() { }

        static Apr()
        {
            int apr_status;
            Debug.Write("apr_initialize...");
            apr_status = apr_initialize();
            Debug.WriteLine("Done");
            if(apr_status != 0)
                throw new AprException(apr_status);

            //FIXME: Shoud shedule a call to apr_terminate 
            //       at process exit        
        }

        public static void Terminate()
        {
            Debug.Write("apr_terminate...");
            apr_terminate();
            Debug.WriteLine("Done");
        }

        #region Initialize / Terminate
        [DllImport(AprLibrary)]
        private static extern int apr_initialize();

        [DllImport(AprLibrary)]
        private static extern void apr_terminate();
        #endregion

        #region Error
        [DllImport(AprLibrary)] // CLSCompliant(false)
        private static extern void apr_strerror(int apr_status,
                                                StringBuilder buf,
                                                uint size);

        public static string StrError(int apr_status)
        {
            StringBuilder buf = new StringBuilder(1024);
            Apr.apr_strerror(apr_status, buf, (uint) buf.Capacity);
            return (buf.ToString());
        }
        #endregion

        #region Allocator
        [DllImport(AprLibrary)]
        static extern
internal int apr_allocator_create(out IntPtr allocator);

        [DllImport(AprLibrary)]
        static extern
internal void apr_allocator_destroy(IntPtr allocator);

        [DllImport(AprLibrary)] /* CLSCompliant(false) */
        static extern
internal IntPtr apr_allocator_alloc(IntPtr allocator,
uint size);

        [DllImport(AprLibrary)]
        static extern
internal void apr_allocator_free(IntPtr allocator,
                 IntPtr memnode);

        [DllImport(AprLibrary)]
        static extern
internal void apr_allocator_owner_set(IntPtr allocator,
                      IntPtr pool);

        [DllImport(AprLibrary)]
        static extern
internal IntPtr apr_allocator_owner_get(IntPtr allocator);

        [DllImport(AprLibrary)] /* CLSCompliant(false) */
        static extern
internal void apr_allocator_max_free_set(IntPtr allocator,
uint size);

        [DllImport(AprLibrary)]
        static extern
internal void apr_allocator_mutex_set(IntPtr allocator,
                      IntPtr mutex);

        [DllImport(AprLibrary)]
        static extern
internal IntPtr apr_allocator_mutex_get(IntPtr allocator);
        #endregion

        #region ThreadMutex
        [DllImport(AprLibrary)] /* CLSCompliant(false) */
        static extern
internal int apr_thread_mutex_create(out IntPtr mutext,
uint flags,
IntPtr pool);

        [DllImport(AprLibrary)]
        static extern
internal int apr_thread_mutex_lock(IntPtr mutex);

        [DllImport(AprLibrary)]
        static extern
internal int apr_thread_mutex_trylock(IntPtr mutex);

        [DllImport(AprLibrary)]
        static extern
internal int apr_thread_mutex_unlock(IntPtr mutex);

        [DllImport(AprLibrary)]
        static extern
internal int apr_thread_mutex_destroy(IntPtr mutex);

        [DllImport(AprLibrary)]
        static extern
internal IntPtr apr_thread_mutex_pool_get(IntPtr mutex);
        #endregion

        #region Pool
        [DllImport(AprLibrary)]
        static extern
internal int apr_pool_create_ex(out IntPtr newpool, IntPtr parent,
                IntPtr abort_fn, IntPtr allocator);

        [DllImport(AprLibrary)]
        static extern
internal void apr_pool_destroy(IntPtr p);

        [DllImport(AprLibrary)]
        static extern
internal IntPtr apr_pool_allocator_get(IntPtr pool);

        [DllImport(AprLibrary)]
        static extern
internal void apr_pool_clear(IntPtr pool);

        [DllImport(AprLibrary)] /* CLSCompliant(false) */
        static extern
internal IntPtr apr_palloc(IntPtr pool, uint size);

        [DllImport(AprLibrary)] /* CLSCompliant(false) */
        static extern
internal IntPtr apr_pcalloc(IntPtr pool, uint size);

        [DllImport(AprLibrary)]
        static extern
internal IntPtr apr_pool_parent_get(IntPtr pool);

        [DllImport(AprLibrary)]
        static extern
internal int apr_pool_is_ancestor(IntPtr a, IntPtr b);
        /*        
                [DllImport(AprLibrary)] static extern
                internal void apr_pool_tag(IntPtr pool, IntPtr tag);
        
                internal delegate int AprPoolCleanUpDelegate(IntPtr data);
        
                [DllImport(AprLibrary)] static extern
                internal int apr_pool_userdata_set(IntPtr data, 
                                                   IntPtr key, 
                                                   AprPoolCleanUpDelegate cleanup,
                                                   IntPtr pool);
                                           
                [DllImport(AprLibrary)] static extern
                internal int apr_pool_userdata_setn(IntPtr data, 
                                                   IntPtr key, 
                                                   AprPoolCleanUpDelegate cleanup,
                                                   IntPtr pool);
                                           
                [DllImport(AprLibrary)] static extern
                internal int apr_pool_userdata_get(out IntPtr data, 
                                                   IntPtr key,
                                                   IntPtr pool);
        
                [DllImport(AprLibrary)] static extern
                internal void apr_pool_cleanup_register(IntPtr pool, 
                                                        IntPtr data, 
                                                        AprPoolCleanUpDelegate plaincleanup,
                                                        AprPoolCleanUpDelegate childcleanup);

                [DllImport(AprLibrary)] static extern
                internal void apr_pool_cleanup_kill(IntPtr pool,
                                                    IntPtr data,
                                                    AprPoolCleanUpDelegate cleanup);
                                            
                [DllImport(AprLibrary)] static extern
                internal void apr_pool_child_cleanup_set(IntPtr pool, 
                                                         IntPtr data, 
                                                        AprPoolCleanUpDelegate plaincleanup,
                                                        AprPoolCleanUpDelegate childcleanup);
                                                
                [DllImport(AprLibrary)] static extern
                internal int apr_pool_cleanup_run(IntPtr pool,
                                                  IntPtr data,
                                                  AprPoolCleanUpDelegate cleanup);

                [DllImport(AprLibrary)] static extern
                internal int apr_pool_cleanup_null(IntPtr data);
        
                [DllImport(AprLibrary)] static extern
                internal void apr_pool_cleanup_for_exec();
        */
        #endregion

        #region AprTime
        [DllImport(AprLibrary)]
        static extern
internal long apr_time_now();

        [DllImport(AprLibrary)]
        static extern
internal int apr_ctime(StringBuilder date_str, long input);

        [DllImport(AprLibrary)]
        static extern
internal int apr_rfc822_date(StringBuilder date_str, long input);
        #endregion

        #region AprTimeExp
        [DllImport(AprLibrary)]
        static extern
internal int apr_time_exp_gmt(IntPtr result, long input);

        [DllImport(AprLibrary)]
        static extern
internal int apr_time_exp_gmt_get(out long result, IntPtr input);

        [DllImport(AprLibrary)]
        static extern
internal int apr_time_exp_get(out long result, IntPtr input);

        [DllImport(AprLibrary)]
        static extern
internal int apr_time_exp_lt(IntPtr result, long input);

        [DllImport(AprLibrary)]
        static extern
internal int apr_time_exp_tz(IntPtr result, long input, int offset);

        [DllImport(AprLibrary)] /* CLSCompliant(false) */
        static extern
internal int apr_strftime(StringBuilder s, out uint retsize,
uint maxsize, string Format, IntPtr input);
        #endregion

        #region AprString
        [DllImport(AprLibrary)]
        static extern
internal IntPtr apr_pstrdup(IntPtr pool, IntPtr str);
        [DllImport(AprLibrary, CharSet = CharSet.Ansi)]
        static extern
internal IntPtr apr_pstrdup(IntPtr pool, string str);

        [DllImport(AprLibrary)] /* CLSCompliant(false) */
        static extern
internal IntPtr apr_pstrndup(IntPtr pool, IntPtr str, uint size);
        [DllImport(AprLibrary, CharSet = CharSet.Ansi)] /* CLSCompliant(false) */
        static extern
internal IntPtr apr_pstrndup(IntPtr pool, string str, uint size);
        #endregion

        #region AprHash
        [DllImport(AprLibrary)]
        static extern
internal IntPtr apr_hash_make(IntPtr pool);

        [DllImport(AprLibrary)]
        static extern
internal IntPtr apr_hash_copy(IntPtr pool, IntPtr h);

        [DllImport(AprLibrary)]
        static extern
internal void apr_hash_set(IntPtr ht, IntPtr key, int klen, IntPtr val);

        [DllImport(AprLibrary)]
        static extern
internal IntPtr apr_hash_get(IntPtr ht, IntPtr key, int klen);

        [DllImport(AprLibrary)] /* CLSCompliant(false) */
        static extern
internal uint apr_hash_count(IntPtr ht);

        [DllImport(AprLibrary)]
        static extern
internal IntPtr apr_hash_overlay(IntPtr p, IntPtr overlayh, IntPtr baseh);

        [DllImport(AprLibrary)]
        static extern
internal IntPtr apr_hash_pool_get(IntPtr thehash);

        [DllImport(AprLibrary)]
        static extern
internal IntPtr apr_hash_first(IntPtr p, IntPtr ht);

        [DllImport(AprLibrary)]
        static extern
internal IntPtr apr_hash_next(IntPtr hi);

        [DllImport(AprLibrary)]
        static extern
internal void apr_hash_this(IntPtr hi, out IntPtr key, out int klen, out IntPtr val);
        #endregion

        #region AprHash
        [DllImport(AprLibrary)]
        static extern
internal IntPtr apr_array_make(IntPtr pool, int elts, int elt_size);

        [DllImport(AprLibrary)]
        static extern
internal IntPtr apr_array_copy(IntPtr pool, IntPtr arr);

        [DllImport(AprLibrary)]
        static extern
internal IntPtr apr_array_copy_hdr(IntPtr pool, IntPtr arr);

        [DllImport(AprLibrary)]
        static extern
internal IntPtr apr_array_append(IntPtr pool, IntPtr first, IntPtr second);

        [DllImport(AprLibrary)]
        static extern
internal void apr_array_cat(IntPtr dst, IntPtr src);

        [DllImport(AprLibrary, CharSet = CharSet.Ansi)]
        static extern
internal IntPtr apr_array_pstrcat(IntPtr pool, IntPtr arr, char sep);

        [DllImport(AprLibrary)]
        static extern
internal IntPtr apr_array_push(IntPtr arr);

        [DllImport(AprLibrary)]
        static extern
internal IntPtr apr_array_pop(IntPtr arr);

        [DllImport(AprLibrary)]
        static extern
internal bool apr_is_empty_array(IntPtr arr);
        #endregion

        #region AprFile
        [DllImport(AprLibrary, CharSet = CharSet.Ansi)]
        static extern
internal int apr_file_open(out IntPtr new_file, string fname,
int flag, int perm, IntPtr pool);

        [DllImport(AprLibrary)]
        static extern
internal int apr_file_close(IntPtr file);

        [DllImport(AprLibrary)]
        static extern
internal int apr_file_open_stdin(out IntPtr thefile, IntPtr pool);

        [DllImport(AprLibrary)]
        static extern
internal int apr_file_open_stdout(out IntPtr thefile, IntPtr pool);

        [DllImport(AprLibrary)]
        static extern
internal int apr_file_open_stderr(out IntPtr thefile, IntPtr pool);

        [DllImport(AprLibrary)]
        static extern internal int apr_file_pipe_create(out IntPtr input, out IntPtr output, IntPtr pool);

        [DllImport(AprLibrary)]
        static extern internal int apr_file_read(IntPtr file, byte[] buf, ref int size);

        [DllImport(AprLibrary)]
        static extern internal int apr_file_write(IntPtr file, byte[] buf, ref int size);

        [DllImport(AprLibrary)]
        static extern internal int apr_file_flush(IntPtr file);

        [DllImport(AprLibrary)]
        static extern internal int apr_file_eof(IntPtr file);

        #endregion
    }
}

