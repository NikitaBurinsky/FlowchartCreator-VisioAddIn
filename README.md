# FlowchartCreator-VisioAddIn

An advanced MS Visio COM Add-In designed to automatically generate structured flowcharts directly from C/C++ source code.

![image](https://github.com/user-attachments/assets/692d85c8-7b39-4ead-9db0-0e562fa87387)

---

## Key Features

### 1. VSTO Ribbon Integration
- **On-Demand Launch**: Visio loads instantly and cleanly without any blocking startup popups. The generator can be launched anytime by navigating to the new **"Flowchart Creator"** tab and clicking **"Create Flowchart"**.
- **Clean Document Spawning**: Generates flowcharts on a clean new Visio document automatically, leaving your existing drawings untouched.

### 2. Advanced C/C++ Code Editor
- **Dynamic Line Numbers**: Gutter with real-time scroll-tracked line numbers.
- **Syntax Highlighting**: Flicker-free coloring for C keywords (blue), strings (dark red), and comments (green).
- **Auto-Indent**: Automatically matches the indentation (tabs/spaces) of the previous line when pressing `Enter`.
- **Bracket Auto-Close**: Inserts matching pairs for `{}`, `()`, `[]`, `""`, and `''`. Supports wrapping active text selections.
- **Font Zooming**: Adjust editor text size using `Ctrl + Plus` / `Ctrl + Minus` or standard `Ctrl + MouseWheel`.
- **Theme Selection**: Toggle between high-contrast **Light** and **Dark** editor themes.
- **Editor Hotkeys**:
  - `Ctrl + /`: Toggle line comments (`//`) on selected block.
  - `Tab` / `Shift + Tab`: Indent or outdent selected blocks of code.

### 3. Loop Visualisation
- Correct rendering of pre-test and post-test loop boundaries (`while`, `for`, `do-while`).
- Automatically routes a loop-back feedback connector from the bottom of the loop body back to the top condition block.

### 4. Interactive Pre-Validation & Error Handling
- **Static Validator**: Automatically counts brace sets (`{}` and `()`) before parsing. Shows a warning dialog if a mismatch is found, preventing broken generator runs.
- **Friendly Error Dialog (`ErrorForm`)**: Replaced legacy message box crashes with a structured collapsible error reporter. Categorizes parser exceptions, offers quick advice, and includes a **"Copy Log"** button to easily send logs to developers.
- **Self-Healing Config**: Automatically checks and recreates `Commands.json` configuration folder and files if missing.

---

## Token Mapping

The generator scans the C/C++ code, classifying statements into structural tokens:
- `IF`, `ELSEIF`, `ELSE` (Conditional diamonds)
- `SWITCH`, `CASE`, `DEFAULT_SWITCH` (Switch branches)
- `LOOP` (for, while block start), `DO_LOOP` (do-while start), `END_LOOP` (loop boundary bottom)
- `PROCESS` (basic assignments, operations)
- `SUBPROCESS` (functions without special tag)
- `INPUT` (e.g. `scanf`, `getchar`, `cin`)
- `OUTPUT` (e.g. `printf`, `putchar`, `cout`)
- `StartFunc` (Entry point shape)
- `RETURN` / `BREAK` / `IGNORE`

---

## Customizing Command Classification

Custom function signatures and command keywords are stored in `%localappdata%\FlowchartCreatorAddIn\Commands.json`. You can open this configuration file directly from the add-in form using the **"Commands file"** button.

### Format Example:
```json
{
  "printf": "OUTPUT",
  "scanf": "INPUT",
  "myCustomCalculation": "PROCESS"
}
```
If a function call is not registered in `Commands.json`, it defaults to `SUBPROCESS` or `PROCESS`.

---

## Screenshots

![image](https://github.com/user-attachments/assets/82ae3fe9-6fb4-49e1-b120-567fb97f5dc8)
