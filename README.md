# Second Brain Architecture
### A local, context-aware personal assistant — built to think with you, not for you.

---

## What is this?

Second Brain Architecture is a personal desktop assistant that runs entirely on your own machine.
It watches what you are working on, understands the context, searches your personal knowledge
base, and surfaces the most relevant notes, goals, and information — exactly when you need them,
without you having to remember where you saved anything.

The core idea is simple: instead of relying on your memory to find the right note at the right
moment, the assistant does that work quietly in the background, and steps in only when it can
genuinely help.

---

## How it works

When you launch the app, it detects what you are doing — which application is open, which file
you are working on, what topic you are researching — and automatically selects the most relevant
mode. You can also choose or override the mode manually at any time.

A small side panel appears alongside your work. It greets you, shows what mode you are in, and
offers to surface relevant notes or remind you of previously saved information. It is not
intrusive — it responds to a shortcut and stays out of your way until you need it.

If you are writing code in C#, it will offer C# notes. If you switch to SQL, it switches with
you. If you are researching something new, it detects that too, identifies the question you are
trying to answer, and helps you save the result in a structured way so you can find it again.

---

## Knowledge base structure

Your notes are stored locally in plain Markdown files, organised by mode and topic:

/Knowledge
  /Coding
    /CSharp/
    /SQL/
    /Python/
    /New/
  /Tasks/
  /Ideas/
  /General/
/Goals/
/Important/

Every note contains a small metadata block at the top that tells the assistant what it is about,
which mode it belongs to, and how important it is. Two flags drive how aggressively the assistant
surfaces a note:

- Hard to remember — things you tend to forget and want proactively shown.
- Important — things that matter even when you remember them well.

---

## Note lifecycle

Notes are not static. The assistant tracks when you last used each one and adjusts accordingly:

- Active (used in last 30 days) — shown normally in search results.
- Archived (30–90 days unused) — hidden from normal results but searchable on request.
- Compressed (90+ days unused) — summarised to key points only, full note preserved and expandable.

Nothing is ever deleted. Old knowledge is compressed and kept, ready to be expanded the moment
you need it again.

---

## Goals and important information

Two files sit outside the general notes and are treated with higher priority:

- Goals — tracked with a status, progress percentage, and deadline. The assistant surfaces
  these on demand or reminds you of progress.
- Important info — a curated list of things that are hard to remember but critical to have
  on hand, shown prominently when relevant to your current work.

---

## Tech stack

| Layer              | Technology        | Role                                                      |
|--------------------|-------------------|-----------------------------------------------------------|
| Core engine        | C# / .NET 8       | Windows process detection, file watching, database        |
| Knowledge & AI     | Python (FastAPI)  | Semantic search, embeddings, LLM response generation      |
| Local AI model     | Ollama            | Private, free, runs entirely on your machine              |
| Database           | SQLite            | Fast local storage for indexed note metadata              |
| UI (planned)       | TypeScript        | Dashboard and visual interface, later phase               |

---

## Development roadmap

- Phase 0 (in progress) — Project foundation: SQLite schema, note indexer, keyword search.
- Phase 1 — Automatic mode detection from active window and open files.
- Phase 2 — Python service with semantic search (search by meaning, not just keywords).
- Phase 3 — Ollama AI responses: real answers composed from your own notes.
- Phase 4 — UI: system tray app, side panel, dashboard.

---

## Why build this?

Every developer, student, or knowledge worker eventually hits the same problem: you learn
something, save it somewhere, and then spend more time trying to find it again than it would
have taken to just look it up. The goal of this project is to solve that problem permanently —
with a tool that lives on your machine, respects your privacy, costs nothing to run, and gets
smarter the more you use it.

---

*Built with C#, Python, and SQLite. Designed to grow one phase at a time.*
