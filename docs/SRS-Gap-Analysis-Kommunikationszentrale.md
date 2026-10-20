# SRS ↔ Implementation Gap Analysis — Kommunikationszentrale

**Source SRS:** `docs/requirements/SRS - Kommunikationszentrale.md` (235 requirement rows: `FR-xxxx` system + `EFR-xxxx` external)
**Scope analysed:** Back-end (`C:\workKZ\src\`) and the **two external Angular front-ends** referenced by `src/NovaCommunicationService.AppHost/AppHost.cs`:
- **KV — Communication Management:** `C:\nova-communication-management-main` (serves on :4300)
- **KZ — Pharmacy Communication Center:** `C:\kommZFrontEnd\de-adg-pharmacy-communication-center-fe` (serves on :4200)

**Date:** generated from current `main` working tree.

> Legend — ✅ Implemented · 🟡 Partial / verify · ❌ Missing · ➖ N/A (external system / out of scope)

> **Correction note:** an earlier revision concluded the front-end was entirely missing. That was wrong — it only inspected the empty in-repo `C:\workKZ\frontend` scaffold. The real UIs live in the two separate repos above (Angular 22 + Kendo UI + NGXS + Transloco + OpenAPI clients) and are substantially implemented.

---

## 1. Executive summary

| Area | Back-end | Front-end |
| --- | --- | --- |
| Message lifecycle (create/edit/publish/schedule/update/withdraw/delete) | ✅ | ✅ (KV `nachrichten` + dialogs) |
| Archive / unarchive | ✅ | 🟡 (KZ archive view; KV action to confirm) |
| Versioning & history (Kap. 8.8) | ✅ | ✅ (KV history dialog) |
| Recipients | ✅ | ✅ (KV `empfaenger`) |
| Target groups (UC-08/10/11) | ✅ | ✅ (KV `zielgruppen`) |
| Templates (UC-20/22/23) | ✅ | ✅ (KV `vorlagen` + edit) |
| Consumer/KZ consumption (list/show/read-state/favorite/archive) | ✅ | ✅ (KZ `inbox` views) |
| Print (UC-07) | ✅ | 🟡 (buttons present; wiring to confirm) |
| Notifications & follow-up (Wiedervorlage) | ✅ | 🟡 (verify in KZ shell) |
| Metrics / evaluation (Auswertung) | ✅ | ✅ (KV `auswertungen`) |
| ADG Desk answer intake (UC-27) | 🟡 | ✅ (KV `desk-antworten`) |
| **Fine-grained permissions (per-right)** | ❌ | ❌ |
| **Permission management (UC-31, FR-0097–0100)** | ❌ | ❌ (no route) |
| **Protokollierung / audit log (Kap. 8.7)** | ❌ | ➖ (backend concern) |
| **CSV recipient import (FR-0149)** | ❌ | 🟡 (verify KV create dialog) |
| **Newsletter / Vplus** | ❌ | ➖ (out of scope Stufe 1) |
| Warenwirtschaft launch (EFR-0001 / FR-0041) | 🟡 (recipient sync) | ➖ external |

### Cross-cutting findings
1. **Both front-ends exist and are feature-rich** (corrected). KV covers management use cases; KZ covers pharmacy-consumer use cases. Depth assessed from `app.routes.ts` + sampled components (`messages-page.component.ts` wires create/publish/withdraw/delete/update/history dialogs). Component internals not exhaustively line-verified (enumeration tooling was intermittently down) → several 🟡 "present, confirm depth" marks.
2. **Fine-grained authorization still not enforced (back-end).** Only broad `CommunicationRead/Write/Consume`; `HasPermission` never called; `MessagesController` has `TODO: implement permissions`. `FR-0002/0116/0123/0028` → 🟡.
3. **No Protokollierung / audit trail (Kap. 8.7)** back-end. Distinct from **Historie (Kap. 8.8)** which IS implemented (`MessageService.AddHistory`).
4. **Permission management (UC-31, FR-0097–0100)** — no back-end endpoint, no KV route → Missing both tiers.
5. **CSV recipient import (FR-0149)** — no back-end parsing; KV create dialog unverified. Likely a gap.
6. **Newsletter / Vplus** — no code; consistent with SRS "Nicht im Scope von Stufe 1".

---

## 2. Independent functional requirements

| ID | Requirement (short) | Back-end | Front-end | Evidence / Notes |
| --- | --- | --- | --- | --- |
| FR-0028 | Restrict access per Kap. 8.6 | 🟡 | 🟡 | Broad policies; Okta `authGuard` both apps; no per-right |
| FR-0041 | KZ reachable from WWS UI | ➖/🟡 | ➖ | External launch; sync via `PharmacyChangedSubscriber` |
| FR-0024 | Validate mandatory fields before transitions | 🟡 | 🟡 | Backend flow; KV dialogs enforce some fields |
| FR-0025 | Status transitions per Kap. 8.1.2 | ✅ | ✅ | `MessageService`; KV action rail |
| FR-0091–0094 | KZ/KV without browser chrome / NOVA frame | 🟡 | 🟡 | Standalone Angular shells; confirm framing |
| FR-0103 | Restrict actions to status-valid ones | ✅ | ✅ | Backend guards; KV `ActionDef.enabled(m)` |

---

## 3. Use-case coverage

### E2E-KV-01 — Nachricht erstellen und veröffentlichen

#### UC-01: Nachricht erstellen
| ID | Requirement (short) | Back-end | Front-end | Evidence / Notes |
| --- | --- | --- | --- | --- |
| FR-0001 | Create message | ✅ | ✅ | `CreateDraftAsync`; KV `CreateMessageDialogComponent` |
| FR-0004 | Save as draft | ✅ | ✅ | backend + create dialog |
| FR-0005 | New message → „Entwurf" | ✅ | ✅ | backend sets Draft |
| FR-0009 | Prefill title „Neue Nachricht" | 🟡 | 🟡 | no backend default; confirm KV dialog |
| FR-0003 | Create from template | 🟡 | 🟡 | `TemplateService`; KV path to confirm |
| FR-0068 | Edit attributes (Kap. 8.2.1) | ✅ | ✅ | Create/Edit inputs; KV dialog |
| FR-0014 | Choose urgency (Kap. 8.4) | ✅ | ✅ | `Urgency` enum; KV chips/field |
| FR-0021 | Expiry in future | 🟡 | 🟡 | `ExpirationDate`; validation to confirm |
| FR-0022 | Notification duration (Kap. 8.5) | ✅ | 🟡 | backend fields; KV field to confirm |
| FR-0023 | Follow-up interval (Kap. 8.5) | ✅ | 🟡 | `FollowUpIntervalHours`; confirm |
| FR-0148 | Add recipients via IDF | ✅ | 🟡 | backend `Recipients`; KV picker to confirm |
| FR-0149 | Add recipients via CSV | ❌ | 🟡 | no backend CSV; KV import to confirm |
| FR-0016 | Add recipients via target group | ✅ | 🟡 | backend `TargetGroups`; KV picker to confirm |
| FR-0017 | Remove recipients | ✅ | 🟡 | backend edit; KV UI |
| FR-0150 | No duplicate recipient | 🟡 | 🟡 | confirm dedupe |
| FR-0151/FR-0007 | Exclude „Gelöscht" recipients | 🟡 | 🟡 | `RecipientStatus`; confirm |
| FR-0011/0012 | Text formatting / links (Kap. 8.2.4) | 🟡 | 🟡 | stored text; KV rich-text editor to confirm |
| FR-0096 | Images in text | ✅ | 🟡 | `MessageImagesController`; KV upload to confirm |
| FR-0101 | Draft mandatory-field check | 🟡 | 🟡 | partial |
| FR-0002 | Restrict create to authorized users | 🟡 | 🟡 | broad write only |
| FR-0178 | Historize creation | ✅ | ✅ | `AddHistory`; KV history dialog |

#### UC-02: Veröffentlichen
| Capability | Back-end | Front-end | Evidence |
| --- | --- | --- | --- |
| Publish immediately | ✅ | ✅ | `PublishAsync`; KV `PublishDialogComponent` |
| Schedule / unschedule | ✅ | 🟡 | backend `/schedule` `/unschedule`; KV UI to confirm |

> ⚠️ **Scheduled auto-publish trigger** — no backend `BackgroundService`/`IHostedService` → Missing (spot-check confirmed).

#### UC-12/UC-29: Bearbeiten / Löschen
| Capability | Back-end | Front-end | Evidence |
| --- | --- | --- | --- |
| Edit draft (UC-12) | ✅ | ✅ | `EditAsync`; KV edit/update dialog |
| Delete (UC-29) | ✅ | ✅ | `DeleteAsync`; KV `DeleteDialogComponent` |

### E2E-KV-02 — Zielgruppen (UC-08/10/11)
| Capability | Back-end | Front-end | Evidence |
| --- | --- | --- | --- |
| Create/edit target group | ✅ | ✅ | `TargetGroupsController`; KV `zielgruppen` |
| Search/filter | 🟡 | 🟡 | paging; confirm filter |
| Delete | ✅ | ✅ | backend DELETE; KV page |
| Assign recipients | ✅ | 🟡 | `POST {id}/recipients`; KV UI to confirm |

### E2E-KV-03 — Nachträglich steuern

#### UC-13: Aktualisieren (versioned)
| ID | Requirement (short) | Back-end | Front-end | Evidence / Notes |
| --- | --- | --- | --- | --- |
| FR-0035/0038/0053 | Versioning + version date | ✅ | ✅ | `UpdateAsync`; KV `UpdateDialogComponent`; version column |
| FR-0181 | Historize updates | ✅ | ✅ | `AddHistory`; KV history dialog |
| FR-0197–0199 | **Protokollieren** update/recipient changes | ❌ | ➖ | no backend audit |
| FR-0210–0212 | Recipient version/re-add semantics | 🟡 | 🟡 | confirm |
| FR-0256 | Notify newly added recipient | 🟡 | 🟡 | confirm |

#### UC-14: Zurückziehen
| ID | Requirement (short) | Back-end | Front-end | Evidence / Notes |
| --- | --- | --- | --- | --- |
| FR-0115/0117/0118 | Withdraw; status guard; → „Zurückgezogen" | ✅ | ✅ | `WithdrawAsync`; KV `WithdrawDialogComponent` |
| FR-0116 | Restrict to withdraw right | 🟡 | 🟡 | broad write only |
| FR-0119 | Confirm withdrawal | ✅ | ✅ | KV withdraw dialog |
| FR-0120 | **Protokollieren** withdrawal | ❌ | ➖ | no audit |
| FR-0121 | Historize withdrawal | ✅ | ✅ | `AddHistory` |
| FR-0238 | Withdrawal comment | ✅ | ✅ | `WithdrawMessageInput.Reason`; KV field |

#### UC-16: Änderungshistorie einsehen
| ID | Requirement (short) | Back-end | Front-end | Evidence / Notes |
| --- | --- | --- | --- | --- |
| FR-0172 | Show change history | ✅ | ✅ | `GET history`; KV `HistoryDialogComponent` |
| FR-0173–0177 | History fields/order/actor/value changes | 🟡 | 🟡 | confirm completeness |

#### UC-17: Archivieren
| ID | Requirement (short) | Back-end | Front-end | Evidence / Notes |
| --- | --- | --- | --- | --- |
| FR-0122/0124/0126/0127/0128 | Archive/guard/flag/hide/retain | ✅ | 🟡 | `ArchiveAsync`/`UnarchiveAsync`; KZ `archive` view; KV action to confirm |
| FR-0123 | Restrict to archive right | 🟡 | 🟡 | broad write only |
| FR-0125 | Auto-archive on expiry | ❌ | ➖ | no background job |
| FR-0129 | Historize archiving | ✅ | ✅ | `AddHistory` |
| FR-0130 | **Protokollieren** archiving | ❌ | ➖ | no audit |

### E2E-KV-04 — Suchen und anzeigen (UC-18/19)
| Capability | Back-end | Front-end | Evidence |
| --- | --- | --- | --- |
| List/search (status/category/urgency/text) | ✅ | ✅ | `MessagesController.ListAsync`; KV grid + chips + search |
| Show one message (UC-19) | ✅ | ✅ | backend get; KV `MessageDetailComponent` |

### E2E-KV-05 — Vorlagen (UC-20/22/23)
| Capability | Back-end | Front-end | Evidence |
| --- | --- | --- | --- |
| Create/edit template | ✅ | ✅ | `TemplatesController`; KV `vorlagen` + `template-edit` |
| Search/filter | 🟡 | 🟡 | confirm |
| Delete | ✅ | ✅ | backend DELETE; KV page |

### E2E-AD-01 — Antworten aus dem ADG Desk (UC-27)
| ID | Requirement (short) | Back-end | Front-end | Evidence / Notes |
| --- | --- | --- | --- | --- |
| UC-27 | Provide answer from ADG Desk | 🟡 | ✅ | backend intake (`MessageSource`/`DeskReferenceNumber`/`SourceCallUrl`); KV `desk-antworten` |
| EFR-0002–0008 | ADG Desk transmits answer/text/call-no/URL/format/recipient/consent | ➖ | ➖ | External system responsibility |

### E2E-KB — Benachrichtigungen (UC-24/26)
| Capability | Back-end | Front-end | Evidence |
| --- | --- | --- | --- |
| Notifications / follow-up (UC-24) | ✅ | 🟡 | `NotificationsController`; KZ UI to confirm |
| Notification settings (UC-26) | ✅ | 🟡 | `GET/PUT settings`; settings screen to confirm |

### E2E-KZ-01 — Erhalten und nutzen (UC-04/05/07)
| Capability | Back-end | Front-end | Evidence |
| --- | --- | --- | --- |
| List/find in KZ (UC-05) | ✅ | ✅ | `ConsumerMessagesController.ListAsync`; KZ `inbox`/`unread` |
| Show in KZ (UC-04) | ✅ | ✅ | backend get; KZ inbox detail |
| Print (UC-07) | ✅ | 🟡 | backend `/print`; KZ print action to confirm |
| Read status (Lesestatus) | ✅ | ✅ | `MessageReceiptService`; KZ `unread` + mark-read |
| Favorite | ✅ | ✅ | backend favorite; KZ `important`/`saved` |
| Archive view | ✅ | ✅ | KZ `archive` view |

### E2E-KO-01 — Auswerten (Entwurf)
| Capability | Back-end | Front-end | Evidence |
| --- | --- | --- | --- |
| Read-state / reach / evaluations | ✅ | ✅ | `MetricsController`; KV `auswertungen` |

### UC-31 — Berechtigungen verwalten
| ID | Requirement (short) | Back-end | Front-end | Evidence / Notes |
| --- | --- | --- | --- | --- |
| FR-0097–0099 | Grant/revoke/show permissions | ❌ | ❌ | No backend endpoint, no KV route |
| FR-0100 | **Protokollieren** permission changes | ❌ | ➖ | no audit |

---

## 4. External-system requirements (EFR) — appendix

| ID | Owner system | Status in scope | Note |
| --- | --- | --- | --- |
| EFR-0001 | Warenwirtschaft | ➖ external / 🟡 | KZ launch is WWS-side; recipient sync via `PharmacyChangedSubscriber` |
| EFR-0002–0008 | ADG Desk | ➖ external | ADG Desk transmits answer payloads; backend intake shape; KV `desk-antworten` surfaces replies |

---

## 5. Prioritised gaps (recommended focus)

1. **Fine-grained permissions** — `HasPermission` per-right checks (`FR-0002/0116/0123/0028`) + **UC-31 (FR-0097–0100)** on back-end and KV.
2. **Protokollierung / audit log (Kap. 8.7)** — `FR-0100/0120/0130/0197–0199` and all `Protokollierung` rows (back-end).
3. **Scheduled auto-publish** and **auto-archive on expiry (FR-0125)** — back-end background/hosted service (none exists).
4. **CSV recipient import (FR-0149)** — back-end parsing + KV import UI.
5. **Validation & detail confirmations** (🟡 rows): future-expiry (FR-0021), draft mandatory fields (FR-0101), recipient dedupe/status exclusion (FR-0150/0151/0007), history completeness (FR-0173–0177), rich-text/links/images depth (FR-0011/0012/0096), scheduling & notification UI wiring.

---

## 6. Method & caveats

- Requirement inventory from `docs/requirements/SRS - Kommunikationszentrale.md` (235 `FR`/`EFR` rows).
- **Back-end** evidence: `src/Common`, `src/Service`, `src/Infrastructure`, `src/Nova.CommunicationService.Presentation`.
- **Front-end** evidence: `app.routes.ts` of both external apps + sampled feature components (KV `messages-page.component.ts` & dialogs; KZ inbox routes). Component internals not exhaustively line-verified due to intermittent enumeration-tooling outages — hence conservative 🟡 "present, confirm depth" marks.
- 🟡 rows should get a line-level review before being closed.
- All sampled SRS rows are MUSS.
