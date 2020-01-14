using Fpm.MainUI.ViewModels.EmbeddedContent;
using Fpm.ProfileData;
using Fpm.ProfileData.Repositories;
using System.Web.Mvc;

namespace Fpm.MainUI.Controllers
{
    [RoutePrefix("supporting-pages")]
    public class EmbeddedContentController : Controller
    {
        private readonly IDocumentsRepository _documentsRepository;

        public EmbeddedContentController(IDocumentsRepository documentsRepository)
        {
            _documentsRepository = documentsRepository;
        }

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
            var html = _documentsRepository.GetDocumentContent(documentId).Content;

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