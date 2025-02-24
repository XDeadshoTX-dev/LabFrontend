﻿using LabBackend.Blocks.Actions;
using LabBackend.Blocks.Conditions;
using LabBackend.Utils.Abstract;
using LabBackend.Utils.Interfaces;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            if (startBlock.NextBlockId == null &&
                startBlock.TrueBlockId == null &&
                startBlock.FalseBlockId == null)
            {
                result.Add(startBlock);
                return result;
            }

            int newId = 1;

            void Traverse(Block currentBlock)
            {
                if (currentBlock.Type != "if")
                {
                    Block nextBlock = GetBlockById(blocksRAWFrontend, currentBlock.NextBlockId);
                    currentBlock.Id = newId;
                    result.Add(currentBlock);

                    if (currentBlock.Type == "end")
                    {
                        return;
                    }

                    newId++;

                    if (nextBlock != null)
                    {
                        currentBlock.NextBlockId = newId;

                        Traverse(nextBlock);
                    }

                    return;
                }
                else
                {
                    Block trueBlock = GetBlockById(blocksRAWFrontend, currentBlock.TrueBlockId);
                    Block falseBlock = GetBlockById(blocksRAWFrontend, currentBlock.FalseBlockId);

                    currentBlock.Id = newId;
                    newId++;

                    result.Add(currentBlock);

                    if (currentBlock.TrueBlockId != null)
                    {
                        currentBlock.TrueBlockId = newId;
                        Traverse(trueBlock);
                    }
                    if (currentBlock.FalseBlockId != null)
                    {
                        currentBlock.FalseBlockId = newId;
                        Traverse(falseBlock);
                    }

                    return;
                }
            }

            Traverse(startBlock);

            return result;
        }
        public Dictionary<int, Dictionary<int, bool>> CreateAdjacencyMatrix(List<Block> linkedFrontendBlocks)
        {
            Dictionary<int, Dictionary<int, bool>> matrix = new Dictionary<int, Dictionary<int, bool>>();

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
            }

            return matrix;
        }
        public string TranslateCode(string languageCode, List<Block> linkedFrontendBlocks, Dictionary<int, Dictionary<int, bool>> adjacencyMatrix)
        {
            List<string> bufferVariables = new List<string>();
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
                    case "end":
                        backendBlock = new EndBlock(languageCode);
                        break;
                    default:
                        throw new NotSupportedException($"Block type '{frontendBlock.Type}' is not supported.");
                }
                string response = backendBlock.Execute(deep, bufferVariables);

                bufferVariables.Add(response);

                if (frontendBlock.Type == "if")
                {
                    deep++;
                }
                bool isFalsePart = false;
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
                        Traverse(nextId);
                        isFalsePart = true;
                    }
                }
                if (isFalsePart && frontendBlock.Type == "if")
                {
                    deep--;
                }
            }

            Traverse(startId);
            return string.Empty;
        }
    }
}
