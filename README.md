<p align="center">
  <img src="https://github.com/user-attachments/assets/32562bf5-0f0c-4c92-8b55-206fd7c33975" alt="Icon" width="128" height="128">
</p>
<h1 align="center" style="margin: 0;">OmniTools</h1>

<p align="center">
  <img src="https://img.shields.io/badge/Platform-Windows-blue.svg" alt="OmniTools">
  <img src="https://img.shields.io/badge/Version-0.7-orange.svg" alt="OmniTools version">
  <img src="https://img.shields.io/badge/Language-C%23-%23239120" alt="C#">
  <img src="https://img.shields.io/badge/License-MIT-lightgrey.svg" alt="License">
</p>

**OmniTools** est un utilitaire Windows qui facilite l’exécution de scripts variés (installateurs, activateurs, outils de dépannage, etc.). Il vous permet de :

- Sélectionner parmi une liste de scripts/installateurs prédéfinis.
- Désactiver temporairement Windows Defender avant l’exécution d’un script (puis le réactiver).
- Exécuter un script et, optionnellement, redémarrer automatiquement la machine une fois terminé.
- Vérifier si le programme est à jour, télécharger et installer la nouvelle version automatiquement.
- Obtenir des informations système détaillées (version Windows, version .NET, privilèges administrateur, etc.).

> [!CAUTION]
> **Je ne serai pas responsable des dommages causés par l'utilisation d'OmniTools !**

> [!NOTE]
> Cet outil est avant tout conçu pour un usage sous Windows **uniquement**. Son utilisation sous d’autres plateformes n’est pas supportée.

---

## Sommaire

- [🚀 Aperçu du Projet](#apercu-du-projet)
- [⚙️ Fonctionnalités Principales](#fonctionnalites-principales)
- [📌 Prérequis](#prerequis)
- [🛠️ Installation](#installation)
- [🔧 Compilation à partir du code source](#compilation-a-partir-du-code-source)
- [▶️ Utilisation](#utilisation)
  - [🖥️ Lancement de l’Application](#lancement-de-lapplication)
  - [📜 Exécution d’un Script](#execution-dun-script)
  - [🛡️ Désactivation / Réactivation de Windows Defender](#desactivation-reactivation-windows-defender)
  - [🔄 Mises à jour automatiques](#mises-a-jour-automatiques)
  - [🏷️ Informations Système](#informations-systeme)
- [✏️ Personnalisation des Scripts](#personnalisation-des-scripts)
- [📝 Notes du projet](#notes-du-projet)
- [❓ FAQ](#faq)
- [🤝 Contribuer](#contribuer)
- [📜 License](#license)
- [⚠️ Avertissement](#avertissement)
- [📷 Images](#images)
- [👤 Auteur](#auteur)

---

<h2 id="apercu-du-projet">🚀 Aperçu du Projet</h2>

Le projet comporte les classes et formulaires suivants :

- **Program.cs** : Point d’entrée principal, gère la vérification de mise à jour, le lancement de la fenêtre de splash, etc.
- **SplashForm.cs** : Écran de démarrage qui vérifie la connexion Internet, la mise à jour disponible, puis lance la `MainForm`.
- **MainForm.cs** : Fenêtre principale, contenant :
  - Un `ComboBox` pour choisir un script depuis une liste (`ScriptConfig.Scripts`).
  - Des boutons pour exécuter le script, fermer l’application, afficher les infos système, accéder à “À propos”...
  - La possibilité de cocher “Disable Windows Defender before execution” et/ou “Restart PC after execution”.
  - Une zone de logs (`RichTextBox`) permettant d’afficher l’état des opérations.
- **ScriptConfig.cs** : Gère la configuration des scripts à exécuter, notamment via un fichier local `playloads.json` ou, à défaut, une liste par défaut codée en dur.
- **Logger.cs** : Classe statique pour l’écriture des logs en couleur dans la zone prévue à cet effet (`RichTextBox`).
- **AboutFrom.cs** : Fenêtre “À propos” affichant des informations sur OmniTools.
- **SystemInfoForm.cs** : Fenêtre affichant des informations système (Windows, version .NET, droits administrateur...).
- **UpdateForm.cs & UpdateDownloadForm.cs** : Fenêtres gérant l’information et le téléchargement d’une nouvelle version, avec une barre de progression.

L’outil télécharge souvent les scripts directement depuis des dépôts GitHub externes, puis exécute les scripts ainsi obtenus.

---

<h2 id="fonctionnalites-principales">⚙️ Fonctionnalités Principales</h2>

1. **Exécution de Scripts / Installateurs**
   - Permet de télécharger et d’exécuter divers scripts / utilitaires (pilotes, tweak Windows, installateurs Office/Windows, activateurs, etc.).

2. **Désactivation / Réactivation de Windows Defender**
   - Avant l’exécution d’un script (s’il est potentiellement bloqué par Defender).
   - Automatisation de la réactivation post-exécution.

3. **Redémarrage Automatique**
   - Permet de cocher “Restart PC after execution” pour redémarrer Windows une fois le script terminé.

4. **Mises à jour Automatiques**
   - Vérifie si une nouvelle version d’OmniTools est disponible.
   - Télécharge la nouvelle version, ferme l’application en cours et l’installe proprement via un script .bat.

5. **Informations Système**
   - Affiche la version de Windows, la version du .NET Runtime et le statut des privilèges (administrateur ou non).

---

<h2 id="prerequis">📌 Prérequis (Pour le code source)</h2>

- **Windows 7/8.1/10/11** (testé principalement sur Windows 10 et Windows 11).
- **.NET 9.0 (ou version ultérieure)**.
  
  > Vérifiez que vous avez installé le **.NET Desktop Runtime** compatible si besoin.

---

<h2 id="installation">🛠️ Installation</h2>

1. **Télécharger l’exécutable** : Rendez-vous sur la page [Releases](https://github.com/danbenba/OmniTools/releases/latest) pour récupérer la dernière version de `OmniTools.exe`.
2. **Lancer l’exécutable** : Exécutez le fichier `.exe` directement.
   
   > **Conseil** : Exécuter en tant qu’administrateur (clic droit > “Exécuter en tant qu’administrateur”) afin de bénéficier de toutes les fonctionnalités (notamment la désactivation de Windows Defender, modifications dans le registre, etc.).

---

<h2 id="compilation-a-partir-du-code-source">🔧 Compilation à partir du code source</h2>

1. **Cloner le dépôt** :
   
   ```bash
   git clone https://github.com/danbenba/OmniTools.git
   ```
  
2. **Ouvrir la solution** dans Visual Studio ou JetBrains Rider.
3. **Restaurer les packages NuGet** si nécessaire.
4. **Compiler** le projet “OmniTools”.  
   Assurez-vous de cibler `net9.0-windows` ou une version de .NET compatible.  
5. **Exécuter** le projet. La fenêtre `SplashForm` apparaît, puis la `MainForm` s’ouvre.

#### PS: Un fichier `panel.cmd` est disponible pour exécuter et compiler le projet

---

<h2 id="utilisation">▶️ Utilisation</h2>

<h3 id="lancement-de-lapplication">🖥️ Lancement de l’Application</h3>

- Double-cliquez sur `OmniTools.exe` pour démarrer.
- Une **SplashForm** s’affiche, vérifie la connexion et les mises à jour, puis lance la **MainForm**.

<h3 id="execution-dun-script">📜 Exécution d’un Script</h3>

1. Dans le **ComboBox** de la fenêtre principale, sélectionnez un script dans la liste (par exemple *Driver Installer*).
2. (Optionnel) Cochez **“Disable Windows Defender before execution”** si vous pensez que Windows Defender bloquera l’outil.
3. (Optionnel) Cochez **“Restart PC after execution”** si vous voulez que l'ordinateur redémarre juste après.
4. Cliquez sur **Execute**.  
   - OmniTools télécharge le script / programme dans votre dossier `%TEMP%`.  
   - Vous verrez une barre de progression de téléchargement.  
   - Une fois le téléchargement terminé, le programme s’exécutera.

<h3 id="desactivation-reactivation-windows-defender">🛡️ Désactivation / Réactivation de Windows Defender</h3>

- Si l’option est cochée, OmniTools va modifier certaines clés de registre pour **désactiver** Defender juste avant l’exécution du script, puis le **réactiver** ensuite automatiquement.
- **NB** : Nécessite les privilèges administrateur pour fonctionner correctement.

<h3 id="mises-a-jour-automatiques">🔄 Mises à jour automatiques</h3>

1. Au lancement, OmniTools compare sa **version actuelle** avec la version hébergée sur GitHub.
2. Si une nouvelle version est disponible, une **fenêtre de mise à jour** apparaît.
3. Vous pouvez choisir de mettre à jour automatiquement.  
   - OmniTools télécharge le nouvel exécutable dans un fichier temporaire.
   - Puis il lance un petit script `.bat` pour **remplacer** l’ancienne version et démarrer la nouvelle.

<h3 id="informations-systeme">🏷️ Informations Système</h3>

- Le bouton **“System Info”** ouvre une fenêtre récapitulative :
  - Version de Windows (Release, Build)
  - Version du .NET Runtime
  - Mode Administrateur ou non

---

<h2 id="personnalisation-des-scripts">✏️ Personnalisation des Scripts</h2>

OmniTools charge la liste de scripts depuis deux endroits :
1. **Fichier `playloads.json`** (format JSON) placé dans le même dossier que `OmniTools.exe`.
2. À défaut, une **liste par défaut** est chargée depuis `ScriptConfig.cs`.

Vous pouvez donc éditer ou créer votre propre `playloads.json` :

```jsonc
[
  {
    "DisplayName": "Mon Script Perso",
    "DownloadUrl": "https://exemple.com/mon-script.bat",
    "LocalFileName": "mon-script.OmniTools.bat",
    "DefaultArguments": "-myArgs",
    "IsEnabled": true
  },
  // ...
]
```

---

<h2 id="notes-du-projet">📝 Notes du projet</h2>

Ce projet a été développé sur une période de deux mois. Il intègre des scripts tiers créés par d'autres développeurs, tels que *MAS* ou *DriverInstaller* de *PortableApps*, ainsi que plusieurs autres outils.  

En complément, j’ai également conçu des scripts pour adapter certains programmes à *OmniTools*. Ces scripts peuvent être entièrement personnalisés et créés par moi-même, ou bien servir à faciliter l’installation de logiciels existants. Par exemple, pour *Windows Tweaker*, *OmniTools* télécharge automatiquement un fichier `.ps1` et l’adapte pour assurer une compatibilité optimale.  

*OmniTools* est le successeur de *WinActTool*, qui souffrait de nombreux problèmes : manque d’optimisation, bugs fréquents et absence de code open-source. Cette nouvelle version est bien plus stable et performante, une véritable évolution (WinActTool 4.0). 🚀  

### 🖇️ **Liens vers les scripts d'origine**  

- [🌐 UniGetUI](https://github.com/marticliment/UniGetUI) – Téléchargeur d’applications pour Windows (Winget, Choco) 
- [🔌 Rufus](https://github.com/pbatard/rufus) – Outil fiable pour formater les clés USB 
- [🔑 Windows KeyGen](https://github.com/Endermanch/XPKeygen) – Un generateur de cléé de produit pour windows 98 2003 et XP
- [🔄 Windows Update Blocker](https://www.sordum.org/9470/windows-update-blocker-v1-8/) – Gestionnaire de mises à jour Windows


### 🔗 **Liens des scripts adaptés pour OmniTools**  

- [🔧 Troubleshoot (RepairKit)](https://github.com/danbenba/OmniTools.Troubleshoot) – Outil de réparation Windows  
- [⚙️ Windows Tweaks (Winutil)](https://github.com/danbenba/OmniTools.WindowsTweaks) – Outil de personnalisation et d’optimisation de Windows  
- [📥 DriverEasy Professional (PortableApps)](https://github.com/danbenba/OmniTools.DriverInstaller) – Outil d’installation automatique des pilotes  

### 📝 **Liens des scripts que j’ai développés**  

- [🖥️ Edition Changer (MAS + Mon GUI)](https://github.com/danbenba/OmniTools.EditionChanger) – Outil de modification d’édition de Windows ou Office  
- [🔑 Activators (MAS + Mon GUI)](https://github.com/danbenba/OmniTools.Activators) – Outil d’activation de Windows et Office  
- [📀 Media Windows Downloader](https://github.com/danbenba/MediaWinDownloader) – Outil de téléchargement et installation d’ISO Windows  

> [!NOTE]
> Je n’ai pas encore détaillé le fonctionnement de chaque script dans un fichier *README.md*, car cela me prendrait du temps supplémentaire. Toutefois, ces scripts restent assez simples à comprendre et à utiliser.

---

<h2 id="faq">❓ FAQ</h2>

1. **Pourquoi l’antivirus alerte ?**  
   - Certains scripts ou utilitaires (activateurs, tweaks) peuvent être vus comme suspects. C’est pourquoi l’option “Disable Defender” est parfois nécessaire.

2. **Puis-je ajouter mes propres scripts ?**  
   - Oui, via le fichier `playloads.json` ou en modifiant `ScriptConfig.cs`.

3. **Le programme se ferme après la mise à jour, c’est normal ?**  
   - Oui, OmniTools se ferme pour que le fichier `.exe` ne soit pas en cours d’utilisation lors du remplacement.

4. **La désactivation de Windows Defender échoue. Que faire ?**  
   - Assurez-vous d’être **en mode administrateur**. Sans privilèges, les modifications du registre ne seront pas effectives.

---

<h2 id="contribuer">🤝 Contribuer</h2>

Les contributions sont les bienvenues !  
- **Forkez** le projet.
- Créez une **branche** pour vos modifications.
- Ouvrez une **Pull Request** lorsque votre contribution est prête à être fusionnée.

Signalez également toute idée, bug ou suggestion via la section [Issues](https://github.com/danbenba/OmniTools/issues).

---

<h2 id="license">📜 License</h2>

Ce projet est distribué sous la licence **MIT**. Consultez le fichier [LICENSE](LICENSE) pour plus de détails.

---

<h2 id="avertissement">⚠️ Avertissement</h2>

- **Responsabilité** : L’utilisation des scripts (activateurs, etc.) peut être contraire aux EULA de Microsoft ou d’autres éditeurs. Vous êtes **seul responsable** de l’usage que vous en faites.
- **Test et validation** : Certains scripts sont considérés “instables” ou “non officiels”. Utilisez-les à vos risques et périls.

---

<h2 id="images">📷 Images</h2>

![image0](https://github.com/user-attachments/assets/b105e051-4589-4184-be54-147730778855)
![image1](https://github.com/user-attachments/assets/92bc16dd-5b96-4711-94d3-3770820b2712)
![image2](https://github.com/user-attachments/assets/fa32b575-9b43-4968-8638-276ed6ced3d9)
![image](https://github.com/user-attachments/assets/c52b76df-8008-4bf4-9f2b-af0593a5a2d3)

---

<h2 id="auteur">👤 Auteur</h2>

Développé par **[danbenba](https://github.com/danbenba)**.  
Retrouvez le dépôt sur GitHub : [OmniTools](https://github.com/danbenba/OmniTools)

> Si vous trouvez cet outil utile, n’hésitez pas à laisser une étoile ⭐ sur [le dépôt GitHub](https://github.com/danbenba/OmniTools).

Merci d’avoir choisi OmniTools !
