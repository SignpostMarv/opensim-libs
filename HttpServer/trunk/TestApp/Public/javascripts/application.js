// this method will rewrite the onclick url.
function ajaxRequest(link)
{
   new Ajax.Request(link.href);
   return false; 
}
function ajaxUpdater(link, targetId)
{
    new Ajax.Updater(targetId, link.href);
   return false; 
}

Event.observe(window, 'load',
      function() {
        document.getElementsByClassName('modal').each(function(link){  new Control.Modal(link);  });
        document.getElementsByClassName('remoteform').each(function(form){ Event.observe(form,'submit',remoteSubmit.bind(this, form),false) });
        });
        
function remoteSubmit(frm, ev)
{
    frm.request();
   Event.stop(ev);
}