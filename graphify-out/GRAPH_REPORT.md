# Graph Report - .  (2026-07-26)

## Corpus Check
- Corpus is ~1,679 words - fits in a single context window. You may not need a graph.

## Summary
- 20 nodes · 22 edges · 6 communities (3 shown, 3 thin omitted)
- Extraction: 86% EXTRACTED · 9% INFERRED · 5% AMBIGUOUS · INFERRED: 2 edges (avg confidence: 0.9)
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- AI Setup and Security
- Graphify Workflow
- Test Generation Setup
- C# Project Structure
- Project Overview
- Shell Bootstrap

## God Nodes (most connected - your core abstractions)
1. `AI Test Generation` - 6 edges
2. `Setup Requirements Guide` - 5 edges
3. `Graphify Guidance` - 3 edges
4. `Playwright Multi-language Demo with AI Test Generation` - 3 edges
5. `OpenAI API Key` - 3 edges
6. `Environment Variables` - 3 edges
7. `C# AI Test Generation Framework` - 3 edges
8. `Generated Tests` - 3 edges
9. `Security-First` - 2 edges
10. `Fallback Support` - 2 edges

## Surprising Connections (you probably didn't know these)
- `AI Test Generation` --semantically_similar_to--> `C# AI Test Generation Framework`  [INFERRED] [semantically similar]
  README.md → SETUP_REQUIREMENTS.txt
- `Fallback Support` --semantically_similar_to--> `Generated Tests`  [AMBIGUOUS] [semantically similar]
  README.md → SETUP_REQUIREMENTS.txt
- `Playwright Multi-language Demo with AI Test Generation` --references--> `Setup Requirements Guide`  [EXTRACTED]
  README.md → SETUP_REQUIREMENTS.txt
- `Environment Variables` --rationale_for--> `Security-First`  [INFERRED]
  SETUP_REQUIREMENTS.txt → README.md
- `AI Test Generation` --references--> `OpenAI API Key`  [EXTRACTED]
  README.md → SETUP_REQUIREMENTS.txt

## Import Cycles
- None detected.

## Hyperedges (group relationships)
- **Graphify Command Suite** — _github_copilot_instructions_graphify_query, _github_copilot_instructions_graphify_path, _github_copilot_instructions_graphify_explain [EXTRACTED 1.00]
- **Project Feature Set** — readme_multilanguage_playwright_testing, readme_ai_test_generation, readme_api_discovery, readme_security_first, readme_fallback_support [EXTRACTED 1.00]
- **AI Test Generation Setup Workflow** — setup_requirements_openai_api_key, setup_requirements_environment_variables, setup_requirements_csharp_ai_framework, setup_requirements_generated_tests [EXTRACTED 1.00]

## Communities (6 total, 3 thin omitted)

### Community 0 - "AI Setup and Security"
Cohesion: 0.50
Nodes (5): AI Test Generation, API Discovery, Security-First, Environment Variables, OpenAI API Key

### Community 1 - "Graphify Workflow"
Cohesion: 0.50
Nodes (4): Graphify Guidance, Graphify Explain Command, Graphify Path Command, Graphify Query Command

### Community 2 - "Test Generation Setup"
Cohesion: 0.67
Nodes (4): Fallback Support, C# AI Test Generation Framework, Generated Tests, Setup Requirements Guide

## Ambiguous Edges - Review These
- `Fallback Support` → `Generated Tests`  [AMBIGUOUS]
  README.md · relation: semantically_similar_to

## Knowledge Gaps
- **7 isolated node(s):** `csharp-playwright`, `setup.sh script`, `Graphify Query Command`, `Graphify Path Command`, `Graphify Explain Command` (+2 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **3 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **What is the exact relationship between `Fallback Support` and `Generated Tests`?**
  _Edge tagged AMBIGUOUS (relation: semantically_similar_to) - confidence is low._
- **Why does `AI Test Generation` connect `AI Setup and Security` to `Test Generation Setup`, `Project Overview`?**
  _High betweenness centrality (0.122) - this node is a cross-community bridge._
- **Why does `Setup Requirements Guide` connect `Test Generation Setup` to `AI Setup and Security`, `Project Overview`?**
  _High betweenness centrality (0.059) - this node is a cross-community bridge._
- **Why does `Playwright Multi-language Demo with AI Test Generation` connect `Project Overview` to `AI Setup and Security`, `Test Generation Setup`?**
  _High betweenness centrality (0.057) - this node is a cross-community bridge._
- **What connects `csharp-playwright`, `setup.sh script`, `Graphify Query Command` to the rest of the system?**
  _7 weakly-connected nodes found - possible documentation gaps or missing edges._