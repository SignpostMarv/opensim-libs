using System;
using System.Collections.Specialized;

namespace HttpServer.Helpers
{
    public static class WebHelper
    {
        /// <summary>
        /// Used to let the website use different javascript libraries.
        /// Default is <see cref="PrototypeImp"/>
        /// </summary>
        public static JavascriptImplementation JSImplementation = new PrototypeImp();

        /// <summary>
        /// Requests a url through ajax
        /// </summary>
        /// <param name="url">url to fetch</param>
        /// <param name="title">link title</param>
        /// <param name="options">optional options in format "key, value, key, value"</param>
        /// <returns>a link tag</returns>
        public static string AjaxRequest(string url, string title, params string[] options)
        {
            return JSImplementation.AjaxRequest(url, title, options);
        }

        /// <summary>
        /// Ajax requests that updates an element with
        /// the fetched content
        /// </summary>
        /// <param name="url">Url to fetch content from</param>
        /// <param name="title">link title</param>
        /// <param name="targetId">element to update</param>
        /// <param name="options">optional options in format "key, value, key, value"</param>
        /// <returns>A link tag.</returns>
        public static string AjaxUpdater(string url, string title, string targetId, params string[] options)
        {
            return JSImplementation.AjaxUpdater(url, title, targetId, options);
        }

        /// <summary>
        /// A link that pop ups a Dialog (overlay div)
        /// </summary>
        /// <param name="url">url to contents of dialog</param>
        /// <param name="title">link title</param>
        /// <returns>A "a"-tag that popups a dialog when clicked</returns>
        public static string DialogLink(string url, string title)
        {
            return JSImplementation.DialogLink(url, title);
        }

        /// <summary>
        /// Create a &lt;form&gt; tag.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        /// <param name="isAjax"></param>
        /// <returns></returns>
        public static string FormStart(string name, string action, bool isAjax)
        {
            string formStart = "<form method=\"post\" name=\"" + name + "\" id=\"" + name + "\" action=\"" + action +
                               "\"";
            if (isAjax)
                return
                    formStart +
                    "onsubmit=\"" + JSImplementation.AjaxForm() + "\">";
            else
                return formStart + ">";
        }

        /// <summary>
        /// Create a link tag.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string Link(string url, string title)
        {
            return string.Format("<a href=\"{0}\">{1}</a>", url, title);
        }

        /// <summary>
        /// Render errors into a UL with class "errors"
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static string RenderErrors(NameValueCollection errors)
        {
            if (errors == null || errors.Count == 0)
                return string.Empty;

            string temp = "<ul class=\"errors\">\r\n";
            for (int i = 0; i < errors.Count; ++i)
                temp += "<li>" + errors[i] + "</li>\r\n";

            return temp + "</ul>\r\n";
        }
    }

    public interface JavascriptImplementation
    {
        /// <summary>
        /// onsubmit on forms
        /// </summary>
        /// <returns></returns>
        string AjaxForm();

        /// <summary>
        /// Requests a url through ajax
        /// </summary>
        /// <param name="url">url to fetch</param>
        /// <param name="title">link title</param>
        /// <param name="options">optional options in format "key, value, key, value"</param>
        /// <returns>a link tag</returns>
        string AjaxRequest(string url, string title, params string[] options);

        /// <summary>
        /// Ajax requests that updates an element with
        /// the fetched content
        /// </summary>
        /// <param name="url">Url to fetch content from</param>
        /// <param name="title">link title</param>
        /// <param name="targetId">element to update</param>
        /// <param name="options">optional options in format "key, value, key, value"</param>
        /// <returns>A link tag.</returns>
        string AjaxUpdater(string url, string title, string targetId, params string[] options);

        /// <summary>
        /// A link that pop ups a Dialog (overlay div)
        /// </summary>
        /// <param name="url">url to contents of dialog</param>
        /// <param name="title">link title</param>
        /// <returns>A "a"-tag that popups a dialog when clicked</returns>
        string DialogLink(string url, string title);
    }

    public class PrototypeImp : JavascriptImplementation
    {
        /// <summary>
        /// Requests a url through ajax
        /// </summary>
        /// <param name="url">url to fetch</param>
        /// <param name="title">link title</param>
        /// <param name="options">optional options in format "key, value, key, value"</param>
        /// <returns>a link tag</returns>
        public string AjaxRequest(string url, string title, params string[] options)
        {
            return "<a href=\"" + url + "\" onclick=\"return ajaxRequest(this);\">" + title + "</a>";
        }

        /// <summary>
        /// Ajax requests that updates an element with
        /// the fetched content
        /// </summary>
        /// <param name="url">Url to fetch content from</param>
        /// <param name="title">link title</param>
        /// <param name="targetId">element to update</param>
        /// <param name="options">optional options in format "key, value, key, value"</param>
        /// <returns>A link tag.</returns>
        public string AjaxUpdater(string url, string title, string targetId, params string[] options)
        {
            return "<a href=\"" + url + "\" onclick=\"return ajaxUpdater(this, '" + targetId + "');\">" + title + "</a>";
        }

        /// <summary>
        /// A link that pop ups a Dialog (overlay div)
        /// </summary>
        /// <param name="url">url to contents of dialog</param>
        /// <param name="title">link title</param>
        /// <returns>
        /// A "a"-tag that popups a dialog when clicked
        /// </returns>
        /// <remarks><para>Requires Control.Modal found here: http://livepipe.net/projects/control_modal/</para>
        /// And the following javascript (load it in application.js):
        /// <code>
        /// Event.observe(window, 'load',
        ///   function() {
        ///     document.getElementsByClassName('modal').each(function(link){  new Control.Modal(link);  });
        ///   }
        /// );
        /// </code>
        /// </remarks>
        public string DialogLink(string url, string title)
        {
            return String.Format("<a class=\"modal\" href=\"{0}\">{1}</a>", url, title);
        }

        #region JavascriptImplementation Members

        public string AjaxForm()
        {
            return
                "new Ajax.Request(this.action, {asynchronous:true, evalScripts:true, parameters:Form.serialize(this)}); return false;";
        }

        #endregion
    }
}