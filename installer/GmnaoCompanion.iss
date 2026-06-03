#define MyAppName      "Gmnao Companion"
#define MyAppVersion   "1.0.0"
#define MyAppPublisher "gmnao"
#define MyAppExeName   "GmnaoCompanion.exe"
#define MyAppSourceDir "..\GmnaoCompanion\publish"

[Setup]
AppId={{9E2A25A7-B89D-4C9E-9268-2D7D9B1530D3}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}

; Installationsverzeichnis – Nutzer kann es im Wizard ändern
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes

; Output
OutputDir=output
OutputBaseFilename=GmnaoCompanion_Setup_{#MyAppVersion}
SetupIconFile=

; Kompression
Compression=lzma2/ultra64
SolidCompression=yes

; Modernes Aussehen
WizardStyle=modern
; WizardResizable=no  ; in Inno Setup 6 nicht unterstützt

; Nur für den aktuellen Benutzer installieren (kein Admin nötig)
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog

; Sprache
[Languages]
Name: "german"; MessagesFile: "compiler:Languages\German.isl"

; ---------------------------------------------------------------
; Optionale Aufgaben (Checkboxen im Wizard)
; ---------------------------------------------------------------
[Tasks]
Name: "desktopicon"; \
  Description: "Desktop-Verknüpfung erstellen"; \
  GroupDescription: "Zusätzliche Symbole:"; \
  Flags: unchecked

Name: "autostart"; \
  Description: "Gmnao Companion automatisch mit Windows starten"; \
  GroupDescription: "Autostart:"; \
  Flags: checkedonce

; ---------------------------------------------------------------
; Dateien kopieren
; ---------------------------------------------------------------
[Files]
Source: "{#MyAppSourceDir}\{#MyAppExeName}"; \
  DestDir: "{app}"; \
  Flags: ignoreversion

; ---------------------------------------------------------------
; Verknüpfungen
; ---------------------------------------------------------------
[Icons]
; Startmenü
Name: "{autoprograms}\{#MyAppName}"; \
  Filename: "{app}\{#MyAppExeName}"

; Desktop (nur wenn Aufgabe gewählt)
Name: "{autodesktop}\{#MyAppName}"; \
  Filename: "{app}\{#MyAppExeName}"; \
  Tasks: desktopicon

; ---------------------------------------------------------------
; Registry – Autostart (nur wenn Aufgabe gewählt)
; Wird beim Deinstallieren automatisch entfernt
; ---------------------------------------------------------------
[Registry]
Root: HKCU; \
  Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Run"; \
  ValueType: string; \
  ValueName: "GmnaoCompanion"; \
  ValueData: """{app}\{#MyAppExeName}"""; \
  Flags: uninsdeletevalue; \
  Tasks: autostart

; ---------------------------------------------------------------
; Nach der Installation: App starten (optional)
; ---------------------------------------------------------------
[Run]
Filename: "{app}\{#MyAppExeName}"; \
  Description: "{#MyAppName} jetzt starten"; \
  Flags: nowait postinstall skipifsilent

; ---------------------------------------------------------------
; Nachrichten auf Deutsch anpassen
; ---------------------------------------------------------------
[CustomMessages]
german.WelcomeLabel2=Dieser Wizard installiert [name/ver] auf Ihrem Computer.%n%nSchließen Sie alle anderen Anwendungen, bevor Sie fortfahren.
