# Wizard Tower Defence

A 3D grid-based tower defence game developed in Unity and deployed to WebGL. Defend the central fortress against waves of enemies using magical apprentices, manage a two purpose Skill Point economy, and unlock new powers in the skill tree.

- **Playable WebGL Build:** [https://defaultadam.github.io/Wizard-Tower-Defence/](https://defaultadam.github.io/Wizard-Tower-Defence/)
- **GitHub Repository:** [https://github.com/DefaultAdam/Wizard-Tower-Defence](https://github.com/DefaultAdam/Wizard-Tower-Defence)

---

## How to Play

### Game Objective
* **Defend the Fortress:** Enemies walk along the grid paths towards the central fortress.
* **Skill Points (SP):** Your SP pool is both the spending currency and the health of the fortress.
* **Win Condition:** Gain **500 SP** to reach the Ultimate Skill milestone and defeat the enemy wizard.
* **Loss Condition:** If enemy attacks reduce your fortress pool to **0 SP**, the fortress falls and the game is over.

### Controls
| Input | Action |
| :--- | :--- |
| **Left Click** | Select a troop to inspect, place a troop on a valid grid tile, or click UI buttons. |
| **Right Click** / **Escape** | Cancel troop placement or clear the current selection. |
| **Tab** | Open or close the Skill Tree window. |
| **Escape** | Open the pause menu. |
| **R** | Restart the game immediately. |

### Gameplay Guide

1. **Place Your Troops:**
   * Click the **Recruit** button to enter placement mode, then left click any tile that is outlined green.
   * Placing a Level 0 Recruit costs **20 SP**.
   * Tiles display a **green outline** for valid placement and a **red outline** for invalid tiles (such as paths or occupied cells).
   * Your allowed troop count increases as the waves progress (`Wave + 4`, up to a maximum of 25).

2. **Manage Your SP Pool:**
   * You begin each game with **100 SP**.
   * Defeating the different types of enemies awards SP, which increases your SP pool for health and funding upgrades.
   * If an enemy reaches the fortress, it deducts SP directly from your pool.

3. **Inspect and Upgrade Troops:**
   * Left click any placed troop on the field to see its combat stats.
   * Once you reach the SP milestone and unlock the path in the skill tree, you can upgrade placed units on the field into specialised branches:
     * **Level 1 Apprentice:** Requires the 250 SP milestone and costs 100 SP to upgrade.
     * **Level 2 Veteran:** Requires the 350 SP milestone and costs 150 SP to upgrade (or can be placed directly for 150 SP once unlocked).
     * **Level 3 Master:** Requires the 400 SP milestone and costs 200 SP to upgrade.

4. **Unlock Specialisations in the Skill Tree:**
   * Press **Tab** or click the Skill Tree button to open the skill tree window.
   * Spend SP to unlock paths across four magic branches.

5. **Enemy Types:**
   * **Grunt:** Standard enemies that moves along the paths in groups from wave 1.
   * **Sprint:** Fast runner that moves quickly towards the fortress, starting from wave 3.
   * **Juggernaut:** Heavily armoured tank with high health, starting from wave 5.

---

## AI Usage & Source Declaration

AI tools were used in this repository for:

* **CI/CD Pipeline Setup:** AI was used to design and configure the automated GitHub Actions workflow (`.github/workflows/deploy.yml`), including Pull Request validation checks (checking for forbidden cache folders, missing `.meta` files, merge conflict markers, and Unity version mismatches) and automated WebGL build and deployment to GitHub Pages.
* **Code Debugging & Diagnostics:** AI assistance was used as an debugging tool to diagnose compilation errors, resolve Unity C# script issues, and troubleshoot logic edge cases (such as UI raycast interception, grid coordinate conversions, and wave spawn sequencing).
* **Pull Request Descriptions & Commit Formatting:** AI assistance was used to help write Pull Request descriptions, summarise iterative changes, and ensure consistent message formatting across development branches.


---

## Prerequisites

Before opening the project locally, ensure you have installed:

1. **Unity Hub**
2. **Unity Editor 2023.1.22f1**
   * In Unity Hub, install version **`2023.1.22f1`**.
   * Under installed modules, make sure to check **WebGL Build Support**.
   > **Note:** The CI pipeline enforces exact version `2023.1.22f1`. Commits with mismatched versions will fail CI validation checks.
3. **Git**
4. **Code Editor:** Visual Studio, Visual Studio Code (with C# Dev Kit), or JetBrains Rider.

