using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Fpm.MainUI.ViewModels.EmbeddedContent;
using Fpm.ProfileData;
using Fpm.ProfileData.Repositories;

namespace Fpm.MainUI.Controllers
{
    [RoutePrefix("supporting-pages")]
    public class EmbeddedContentController : Controller
    {
        [Route("profile-relationships")]
        public ActionResult ProfileRelationships()
        {
            return GetEmbeddedHtmlPage(DocumentIds.ProfileRelationships);
        }

        /// <summary>
        /// Provides a HTML document.
        /// </summary>
        [Route("embedded-document/{documentId}")]
        public void EmbeddedPage(int documentId)
        {
            var html = new DocumentsRepository()
                .GetDocumentContent(documentId).Content;

            Response.OutputStream.Write(html, 0, html.Length);
            Response.OutputStream.Flush();
        }

        private ActionResult GetEmbeddedHtmlPage(int id)
        {

            var page = new EmbeddedHtmlPage
            {
                DocumentId = id
            };

            return View("EmbeddedHtmlPage", page);
        }
    }
}