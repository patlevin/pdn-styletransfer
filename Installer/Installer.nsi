;------------------------------------------------------------------
; MIT LICENSE
;
; Copyright © 2019 Patrick Levin
;
; Permission is hereby granted, free of charge, to any person obtaining a copy
; of this software and associated documentation files (the "Software"), to deal
; in the Software without restriction, including without limitation the rights
; to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
; copies of the Software, and to permit persons to whom the Software is
; furnished to do so, subject to the following conditions:
; 
; The above copyright notice and this permission notice shall be included in all
; copies or substantial portions of the Software.
; 
; THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
; IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
; FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
; AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
; LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
; OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
; SOFTWARE.
;------------------------------------------------------------------

;-------------------------------------------------------------------
; Nullsoft Scriptable Install System installer script.
;
; Contains the script for gathering required and optional components
; and checks the requirements for installation:
; • Paint.NET must be installed
; • a 64-bit operating system is required
;
; Optional components:
; • language files other than English (US)
;-------------------------------------------------------------------

;-------------------------------------------------------------------
;                          General settings

  !define MUI_CUSTOMFUNCTION_GUIINIT myGuiInit

  !include "MUI2.nsh"               ; use modern UI
  !include x64.nsh                  ; x64 support
  !include LogicLib.nsh             ; make scripting bearable

  OutFile StyleTransferEffect.exe

  VIProductVersion 1.0.0.0
  VIAddVersionKey ProductName "Paint.NET Style Transfer Effect Plugin"
  VIAddVersionKey Comments "An AI-based style transfer effect for Paint.NET. Source code available on github.com/patlevin"
  VIAddVersionKey CompanyName "Patrick Levin"
  VIAddVersionKey LegalCopyright "Patrick Levin © 2020"
  VIAddVersionKey FileDescription "Paint.NET Style Transfer Effect Plugin"
  VIAddVersionKey FileVersion 1.0.0.0
  VIAddVersionKey ProductVersion 1.0.0.0
  VIAddVersionKey InternalName "Style Transfer Effect"
  VIAddVersionKey LegalTrademarks ""
  VIAddVersionKey OriginalFilename StyleTransferEffect.exe

  Unicode True
  RequestExecutionLevel admin       ; request administrator priviledges

###################################################################
#                       INTERFACE CONFIGURATION
###################################################################

  !define MUI_HEADERIMAGE
  !define MUI_HEADERIMAGE_BITMAP ..\StyleTransfer\assets\images\installer-header.bmp
  !define MUI_WELCOMEFINISHPAGE_BITMAP ..\StyleTransfer\assets\images\installer-welcome.bmp

;------------------------------------------------------------------
;                              CONSTANTS

  !define RESOURCE_FILE "StyleTransferEffect.resources.dll"
  !define RELEASE_DIR "..\StyleTransfer\StyleTransferEffect\bin\Release"
  !define ASSETS_DIR "..\StyleTransfer\assets"
  !define DEPS_DIR "..\StyleTransfer\dependencies"
  !define PKG_DIR "..\StyleTransfer\packages"

;-------------------------------------------------------------------
;                               PAGES

  !insertmacro MUI_PAGE_WELCOME
  !insertmacro MUI_PAGE_COMPONENTS
  !insertmacro MUI_PAGE_INSTFILES
  !insertmacro MUI_PAGE_FINISH

;-------------------------------------------------------------------
;                             LANGUAGES

  !include languages.nsh            ; localised texts
  !insertmacro MUI_RESERVEFILE_LANGDLL

  Name $(PROD_NAME)

###################################################################
#                           VARIABLES
###################################################################

  Var PDN_DIR

###################################################################
#                           MACROS
###################################################################

!macro ADD_RESOURCE_DLL lang
  SetOutPath $INSTDIR\${lang}
  File ${RELEASE_DIR}\${lang}\${RESOURCE_FILE}
  SetOutPath $INSTDIR
!macroend

###################################################################
#                       INSTALLER SECTIONS
###################################################################

SectionGroup "!$(SEC_REQ)" ReqGroup
  Section "ONNX Runtime (x64)"
    SectionIn RO
    SetOutPath $INSTDIR
    File ${DEPS_DIR}\Microsoft.ML.OnnxRuntime.dll
    File ${DEPS_DIR}\native\x64\onnxruntime.dll
  SectionEnd

  Section ".NET Standard 2.0 Packages"
    SectionIn RO
    SetOutPath $INSTDIR
    File ${PKG_DIR}\System.Buffers.4.5.0\lib\netstandard2.0\System.Buffers.dll
    File ${PKG_DIR}\System.Memory.4.5.3\lib\netstandard2.0\System.Memory.dll
    File ${PKG_DIR}\System.Runtime.CompilerServices.Unsafe.4.7.0\lib\netstandard2.0\System.Runtime.CompilerServices.Unsafe.dll
  SectionEnd

  Section "AI Models"
    SectionIn RO
    SetOutPath $INSTDIR
    File ${ASSETS_DIR}\models\StyleTransferModels.zip
  SectionEnd

  Section "Presets"
    SectionIn RO
    SetOutPath $INSTDIR
    File ${ASSETS_DIR}\presets\StyleTransferPresets.zip
  SectionEnd

  Section "Effect Plugin"
    SectionIn RO
    SetOutPath $INSTDIR
    File ${RELEASE_DIR}\StyleTransferEffect.dll
    File ${RELEASE_DIR}\StyleTransferEffect.dll.config
  SectionEnd
SectionGroupEnd

SectionGroup $(SEC_LANG) LangGroup
  Section $(LANG_gb) SectionGB
    !insertmacro ADD_RESOURCE_DLL "en-GB"
  SectionEnd
  Section $(LANG_de) SectionDE
    !insertmacro ADD_RESOURCE_DLL "de"
  SectionEnd
  Section $(LANG_fr) SectionFR
    !insertmacro ADD_RESOURCE_DLL "fr"
  SectionEnd
  Section $(LANG_es) SectionES_ES
    !insertmacro ADD_RESOURCE_DLL "es-ES"
  SectionEnd
  Section $(LANG_mx) SectionES_MX
    !insertmacro ADD_RESOURCE_DLL "es-MX"
  SectionEnd
  Section $(LANG_pt) SectionPT_PT
    !insertmacro ADD_RESOURCE_DLL "pt-PT"
  SectionEnd
  Section $(LANG_ru) SectionRU
    !insertmacro ADD_RESOURCE_DLL "ru"
  SectionEnd
SectionGroupEnd

  !insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
    !insertmacro MUI_DESCRIPTION_TEXT ${LangGroup} $(DESC_lang)
    !insertmacro MUI_DESCRIPTION_TEXT ${SectionGB} $(DESC_gb)
    !insertmacro MUI_DESCRIPTION_TEXT ${SectionDE} $(DESC_de)
    !insertmacro MUI_DESCRIPTION_TEXT ${SectionFR} $(DESC_fr)
    !insertmacro MUI_DESCRIPTION_TEXT ${SectionES_ES} $(DESC_es)
    !insertmacro MUI_DESCRIPTION_TEXT ${SectionES_MX} $(DESC_mx)
    !insertmacro MUI_DESCRIPTION_TEXT ${SectionPT_PT} $(DESC_pt)
    !insertmacro MUI_DESCRIPTION_TEXT ${SectionRU} $(DESC_ru)
    !insertmacro MUI_DESCRIPTION_TEXT ${ReqGroup} $(DESC_req)
  !insertmacro MUI_FUNCTION_DESCRIPTION_END

###################################################################
#  Fn: IsPaintDotNetInstalled
#
#  Returns whether PDN is installed and sets the PDN folder in
#  $PDN_DIR.
#
#  Usage:
#    Call IsPaintDotNetInstalled
#    Pop $R0 ; "1", if PDN is installed, "0" otherwise
#
###################################################################
Function IsPaintDotNetInstalled
  ClearErrors
  SetRegView 64  ; make 64-bit program keys visible
  ReadRegStr $PDN_DIR HKLM SOFTWARE\paint.net TARGETDIR
  SetRegView LastUsed
  ${If} ${Errors}
    Push 0 ; not found
  ${Else}
    Push 1 ; found
  ${EndIf}
FunctionEnd

###################################################################
#  Cb: onInit
#
#  Peforms language selection.
#
#  Usage:
#    [called on installer start]
#
###################################################################
Function .onInit
  !insertmacro MUI_LANGDLL_DISPLAY
FunctionEnd

###################################################################
#  Cb: onGUIInit
#
#  Checks wether the installer is already running (aborts if so)
#  and finds the PDN installation folder. Is called *after* .onInit
#  and used because localisation only takes effect after .onInit.
#
#  Usage:
#    [called on installer start]
#
###################################################################
Function myGuiInit
  System::Call 'kernel32::CreateMutex(p 0, i 0, t "stp_installer") p .r1 ?e'
  Pop $R0
  StrCmp $R0 0 is_uniq
    MessageBox MB_OK|MB_ICONEXCLAMATION "$(MSG_NODUP)"
    Abort
  is_uniq:
  Call IsPaintDotNetInstalled
  Pop $R0
  ${If} $R0 == 0
    MessageBox MB_OK|MB_ICONEXCLAMATION "$(MSG_NOPDN)"
    Abort
  ${EndIf}
  StrCpy $INSTDIR $PDN_DIR\Effects
  ${IfNot} ${RunningX64}
    MessageBox MB_OK|MB_ICONEXCLAMATION "$(MSG_NOX64)"
    Abort
  ${EndIf}
FunctionEnd