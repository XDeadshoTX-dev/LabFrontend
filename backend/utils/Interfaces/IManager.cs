using LabBackend.Utils.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp2.frontend.blocks;

namespace LabBackend.Utils.Interfaces
{
    public interface IManager
    {
        public List<AbstractBlock> GetLinkedBlocks(List<AbstractBlock> uiBlocks);
        public List<Block> GetLinkedFrontendBlocks(List<Block> blocksRAWFrontend);
        public Block GetBlockById(List<Block> uiBlocks, int? findId);
        public Dictionary<int, Dictionary<int, bool>> CreateAdjacencyMatrix(List<Block> linkedFrontendBlocks);
        public void TranslateCode(string languageCode, List<Block> linkedFrontendBlocks, Dictionary<int, Dictionary<int, bool>> adjacencyMatrix);
        public void TranslateCodeMultithread(string languageCode, List<Block> linkedFrontendBlocks, Dictionary<int, Dictionary<int, bool>> adjacencyMatrix);
    }
}
