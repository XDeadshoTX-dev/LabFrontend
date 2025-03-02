using LabBackend.Blocks.Actions;
using LabBackend.Blocks.Conditions;
using LabBackend.Utils.Abstract;
using LabBackend.Utils.Interfaces;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp2.backend.blocks.Conditions;
using WpfApp2.frontend.blocks;
using WpfApp2.frontend.utils;

namespace LabBackend.Utils
{
    public class BlockManager : IManager
    {
        public List<AbstractBlock> GetLinkedBlocks(List<AbstractBlock> uiBlocks)
        {
            List<AbstractBlock> result = new List<AbstractBlock>();
            HashSet<int> visited = new HashSet<int>();

            AbstractBlock startBlock = null;
            foreach (var block in uiBlocks)
            {
                if (block.GetNameBlock() == "start")
                {
                    startBlock = block;
                    break;
                }
            }

            if (startBlock == null)
            {
                return result;
            }

            void Traverse(AbstractBlock currentBlock)
            {
                if (currentBlock == null || visited.Contains(currentBlock.GetId()))
                {
                    return;
                }

                visited.Add(currentBlock.GetId());
                result.Add(currentBlock);

                foreach (var nextBlock in currentBlock.Next)
                {
                    Traverse(nextBlock);
                }
            }

            Traverse(startBlock);

            return result;
        }
        public Block GetBlockById(List<Block> uiBlocks, int? findId)
        {
            //return uiBlocks
            //            .Select(block => block)
            //            .Where(block => block.Id == findId)
            //            .FirstOrDefault();
            return uiBlocks.Find(block => block.Id == findId);
        }
        public List<Block> GetLinkedFrontendBlocks(List<Block> blocksRAWFrontend)
        {
            Block startBlock = blocksRAWFrontend[0];

            List<Block> result = new List<Block>();
            Stack<Block> buffer = new Stack<Block>();

            if (startBlock.NextBlockId == null &&
                startBlock.TrueBlockId == null &&
                startBlock.FalseBlockId == null)
            {
                result.Add(startBlock);
                return result;
            }

            int newId = 1;
            bool existElse = false;
            bool inFalse = false;

            void Traverse(Block currentBlock)
            {
                currentBlock.Id = newId;
                result.Add(currentBlock);

                if (currentBlock.Type != "if")
                {
                    Block nextBlock = null;

                    if (inFalse == true && currentBlock.Type != "else") // without else
                    {
                        inFalse = false;
                        buffer.Pop().ExitElseBlockId = newId;
                    }
                    else // with else
                    {
                        inFalse = false;
                    }
                    if (currentBlock.NextBlockId != null)
                    {
                        nextBlock = GetBlockById(blocksRAWFrontend, currentBlock.NextBlockId);
                    }
                    else if (currentBlock.ExitElseBlockId != null && existElse == true)
                    {
                        nextBlock = GetBlockById(blocksRAWFrontend, currentBlock.ExitElseBlockId);
                    }
                    else if (currentBlock.ExitElseBlockId != null && existElse == false)
                    {
                        newId++;
                        buffer.Push(currentBlock);
                        inFalse = true;
                        return;
                    }
                    else if (currentBlock.Type == "end")
                    {
                        return;
                    }

                    newId++;

                    if (nextBlock != null)
                    {
                        if (currentBlock.ExitElseBlockId != null && existElse == true)
                        {
                            existElse = false;
                            currentBlock.ExitElseBlockId = newId;
                            buffer.Pop().ExitElseBlockId = newId;
                        }
                        else
                        {
                            currentBlock.NextBlockId = newId;
                        }

                        if (currentBlock.Type == "else")
                        {
                            existElse = true;
                        }

                        Traverse(nextBlock);
                    }

                    return;
                }
                else
                {
                    Block trueBlock = GetBlockById(blocksRAWFrontend, currentBlock.TrueBlockId);
                    Block falseBlock = GetBlockById(blocksRAWFrontend, currentBlock.FalseBlockId);

                    newId++;

                    if (trueBlock != null)
                    {
                        currentBlock.TrueBlockId = newId;
                        Traverse(trueBlock);
                    }
                    if (falseBlock != null)
                    {
                        currentBlock.FalseBlockId = newId;
                        Traverse(falseBlock);
                    }

                }
            }

            Traverse(startBlock);

            return result;
        }
        public Dictionary<int, Dictionary<int, bool>> CreateAdjacencyMatrix(List<Block> linkedFrontendBlocks)
        {
            Dictionary<int, Dictionary<int, bool>> matrix = new Dictionary<int, Dictionary<int, bool>>();
            bool isFalsePart = false;

            foreach (Block block in linkedFrontendBlocks)
            {
                matrix[block.Id] = new Dictionary<int, bool>();
                if (block.NextBlockId.HasValue)
                {
                    matrix[block.Id][block.NextBlockId.Value] = true;
                }
                if (block.TrueBlockId.HasValue)
                {
                    matrix[block.Id][block.TrueBlockId.Value] = true;
                }
                if (block.FalseBlockId.HasValue)
                {
                    matrix[block.Id][block.FalseBlockId.Value] = true;
                }
                if (block.ExitElseBlockId.HasValue)
                {
                    if (isFalsePart)
                    {
                        matrix[block.Id][block.ExitElseBlockId.Value] = true;
                        isFalsePart = false;
                    }
                    else
                    {
                        isFalsePart = true;
                    }
                }
            }

            return matrix;
        }
        public void TranslateCode(string languageCode, List<Block> linkedFrontendBlocks, Dictionary<int, Dictionary<int, bool>> adjacencyMatrix)
        {
            Stack<string> bufferVariables = new Stack<string>();
            int startId = adjacencyMatrix.Keys.First();
            int deep = 0;

            void Traverse(int currentId)
            {
                Block frontendBlock = GetBlockById(linkedFrontendBlocks, currentId);
                AbstractBlock backendBlock;

                switch (frontendBlock.Type)
                {
                    case "start":
                        backendBlock = new StartBlock(languageCode);
                        break;
                    case "AssignmentBlock":
                        backendBlock = new AssignmentBlock(languageCode, frontendBlock.Text);
                        break;
                    case "ConstantAssignmentBlock":
                        backendBlock = new ConstantAssignmentBlock(languageCode, frontendBlock.Text);
                        break;
                    case "InputBlock":
                        backendBlock = new InputBlock(languageCode, frontendBlock.Text);
                        break;
                    case "PrintBlock":
                        backendBlock = new PrintBlock(languageCode, frontendBlock.Text);
                        break;
                    case "if":
                        backendBlock = new ConditionBlock(languageCode, frontendBlock.Text);
                        break;
                    case "else":
                        backendBlock = new ElseBlock(languageCode);
                        break;
                    case "end":
                        backendBlock = new EndBlock(languageCode);
                        break;
                    default:
                        throw new NotSupportedException($"Block type '{frontendBlock.Type}' is not supported.");
                }
                string response = backendBlock.Execute(deep, bufferVariables);

                bufferVariables.Push(response);

                if (frontendBlock.Type == "if"
                    || frontendBlock.Type == "else")
                {
                    deep++;
                }
                

                bool isFalsePart = false;
                bool isElsePart = false;

                if (frontendBlock.Type == "else")
                {
                    isElsePart = true;
                }

                foreach (var nextId in adjacencyMatrix[currentId].Keys)
                {
                    if (isFalsePart)
                    {
                        deep--;

                        Traverse(nextId);
                        isFalsePart = false;
                    }
                    else
                    {
                        if (frontendBlock.ExitElseBlockId != null)
                        {
                            deep--;
                            isElsePart = false;

                            do
                            {
                                bufferVariables.Pop();
                            } while (bufferVariables.Pop() != "ElseBlock success");
                        }

                        Traverse(nextId);
                        isFalsePart = true;
                    }
                }
                if (isElsePart)
                {
                    deep--;
                    isElsePart = false;
                }
                if (isFalsePart && frontendBlock.Type == "if")
                {
                    deep--;
                }
                if (bufferVariables.Count != 0)
                {
                    bufferVariables.Pop();
                }
            }

            Traverse(startId);
        }
        public void TranslateCodeMultithread(string languageCode, List<Block> linkedFrontendBlocks, Dictionary<int, Dictionary<int, bool>> adjacencyMatrix)
        {
            Stack<string> bufferVariables = new Stack<string>();
            int startId = adjacencyMatrix.Keys.First();
            Dictionary<int, Dictionary<int, Stack<string>>> keyValuePairs = new Dictionary<int, Dictionary<int, Stack<string>>>();
            int deep = 0;
            int deepIndex = 0;

            void Traverse(int currentId)
            {
                Block frontendBlock = GetBlockById(linkedFrontendBlocks, currentId);
                AbstractBlock backendBlock;

                switch (frontendBlock.Type)
                {
                    case "start":
                        backendBlock = new StartBlock(languageCode);
                        break;
                    case "AssignmentBlock":
                        backendBlock = new AssignmentBlock(languageCode, frontendBlock.Text);
                        break;
                    case "ConstantAssignmentBlock":
                        backendBlock = new ConstantAssignmentBlock(languageCode, frontendBlock.Text);
                        break;
                    case "InputBlock":
                        backendBlock = new InputBlock(languageCode, frontendBlock.Text);
                        break;
                    case "PrintBlock":
                        backendBlock = new PrintBlock(languageCode, frontendBlock.Text);
                        break;
                    case "if":
                        backendBlock = new ConditionBlock(languageCode, frontendBlock.Text);
                        break;
                    case "else":
                        backendBlock = new ElseBlock(languageCode);
                        break;
                    case "end":
                        backendBlock = new EndBlock(languageCode);
                        break;
                    default:
                        throw new NotSupportedException($"Block type '{frontendBlock.Type}' is not supported.");
                }
                string response = backendBlock.ExecuteValidation(bufferVariables);

                keyValuePairs[deepIndex] = new Dictionary<int, Stack<string>>();
                keyValuePairs[deepIndex][deep] = new Stack<string>(bufferVariables);
                deepIndex++;

                bufferVariables.Push(response);


                if (frontendBlock.Type == "if"
                    || frontendBlock.Type == "else")
                {
                    deep++;
                }


                bool isFalsePart = false;
                bool isElsePart = false;

                if (frontendBlock.Type == "else")
                {
                    isElsePart = true;
                }

                foreach (var nextId in adjacencyMatrix[currentId].Keys)
                {
                    if (isFalsePart)
                    {
                        deep--;

                        Traverse(nextId);
                        isFalsePart = false;
                    }
                    else
                    {
                        if (frontendBlock.ExitElseBlockId != null)
                        {
                            deep--;
                            isElsePart = false;

                            do
                            {
                                bufferVariables.Pop();
                            } while(bufferVariables.Pop() != "ElseBlock success");
                        }

                        Traverse(nextId);
                        isFalsePart = true;
                    }
                }
                if (isElsePart)
                {
                    deep--;
                    isElsePart = false;
                }
                if (isFalsePart && frontendBlock.Type == "if")
                {
                    deep--;
                }
                if (bufferVariables.Count != 0)
                {
                    bufferVariables.Pop();
                }
            }

            Traverse(startId);

            Thread[] threads = new Thread[adjacencyMatrix.Count];

            foreach (var item in keyValuePairs)
            {
                int blockId = item.Key + 1;
                int blockIndex = item.Key;
                int deepBlock = item.Value.Keys.First();
                Stack<string> historyVariables = item.Value.Values.First();

                Block frontendBlock = GetBlockById(linkedFrontendBlocks, blockId);
                AbstractBlock backendBlock;

                switch (frontendBlock.Type)
                {
                    case "start":
                        backendBlock = new StartBlock(languageCode);
                        break;
                    case "AssignmentBlock":
                        backendBlock = new AssignmentBlock(languageCode, frontendBlock.Text);
                        break;
                    case "ConstantAssignmentBlock":
                        backendBlock = new ConstantAssignmentBlock(languageCode, frontendBlock.Text);
                        break;
                    case "InputBlock":
                        backendBlock = new InputBlock(languageCode, frontendBlock.Text);
                        break;
                    case "PrintBlock":
                        backendBlock = new PrintBlock(languageCode, frontendBlock.Text);
                        break;
                    case "if":
                        backendBlock = new ConditionBlock(languageCode, frontendBlock.Text);
                        break;
                    case "else":
                        backendBlock = new ElseBlock(languageCode);
                        break;
                    case "end":
                        backendBlock = new EndBlock(languageCode);
                        break;
                    default:
                        throw new NotSupportedException($"Block type '{frontendBlock.Type}' is not supported.");
                }
                
                threads[blockIndex] = new Thread(() =>
                {
                    backendBlock.Execute(deepBlock, historyVariables);
                });
                threads[blockIndex].Start();
                threads[blockIndex].Join();
            }
        }
    }
}
