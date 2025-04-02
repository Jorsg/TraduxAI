using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraduxAI.Shared.Models;

namespace TraduxAI.Translation.Core.Interfaces
{
	public interface IDocumentRepository
	{
		Task<DocumentProcessResult> SaveDocumentResultAsync(DocumentProcessResult result, string userId);
		Task<IEnumerable<DocumentProcessResult>> GetDocumentResultsAsync(string userId);
		Task<DocumentProcessResult> GetDocumentResultByIdAsync(string resultId);
	}
}
