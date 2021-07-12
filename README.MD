![title]

Form and Questionnaire UI generator for Unity

## Author
* [Rodrigo Pinheiro][githubProfile]

## Table of Contents
- [Author](#author)
- [Table of Contents](#table-of-contents)
- [Installation](#installation)
- [Importing new Questionnaires (JSON Format)](#importing-new-questionnaires-json-format)
  - [Data Structures](#data-structures)
- [Exporting Answers](#exporting-answers)
- [Custom Exporters](#custom-exporters)
- [Settings](#settings)
  - [Localization](#localization)
- [Issues](#issues)

## Installation

In the releases page you should find the unity package for this asset, you can import it that way.

You can also add this repository as a dependency or download the source and place them in your unity project.

After one of the mentioned steps, it should be ready to use.

### Creating a new Questionnaire

* Go to the item menu at the top of the Unity Editor and select 

* Tools > QuestForms > Create Questionnaire Canvas.

* The previous command will create a new canvas in your scene setup for interactions and with a questionnaire manager component. Even though the asset does not prevent this, it is important to only have 1 questionnaire manager in your scene.

* In the questionnaire manager you have a From field, which is a `QF_Questionnaire` array. Here you can add multiple questionnaires and pressing the `Generate UI` button will create the UI for the questionnaires in list order.

## Importing new Questionnaires (JSON Format)

To start using QuestForms you need a questionnaire json file with the extension `.quest` instead of json even though the document may be formatted that way.

Creating your own questionnaire from scratch is easy, but if you have any trouble with the json format and the data structures in use check the `Examples` folder inside the project.

It should be also possible to create an editor for this, following json format rules and the existent data structures.

### Data Structures

All of the data structures to create and read a questionnaire are located in the `QF_Questionnaire` script.

Don't be afraid to change this and suit it to your needs, but be mindful in changing already existent information and adding to the custom Editors

The following structures are used in the json file:

```cs
public enum Layout
{
    Vertical,
    Horizontal
}

public enum QuestionType
{
    Scale,
    Option,
    TextField
}

public enum ScrollType
{
    Scroll,
    SplitToPage
}

[System.Serializable]
public class Page
{
    public string ID;
    public bool containsImage;
    public string header;
    public string instructions;
    public string[] scale;
    public Question[] questions;
    public bool randomizeOrder;
    public ScrollType scrollQuestions;
}

[System.Serializable]
public class Question
{
    public string ID;
    public bool containsImage;
    public string qType; // Question type Scale, Option, TextField
    public string question;
    public bool mandatory;
    public string[] options;
    public Layout optionsLayout;
    public int characterMax = 200;
    public int characterMin = 50;
    public QuestionType type => (QuestionType)Enum.Parse(typeof(QuestionType), qType;
}
```

## Exporting Answers

Exporting a user's answers requires a little setup in the editor side of things.

**Saving Locally**

Go to your questionnaire manager and in the export settings section fill in all fields that apply to your use case. Saving locally can be done using a static file name in build, but this may override previous saves if the game/experience is ran multiple times.

To fix this you should change it via scripting on questionnaire initialization or scene load to your questionnaire scene.

It is also recommended to use persistent data path in your save path variable since the game may not be always on the same computer.

![exportSettings]

**Custom save/Database**

In your questionnaire manager, make sure you are exporting to the file type you want, usually json works best for databases and custom solutions.

In your save script subscribe the data export event thrown by the Questionnaire Manager:

```cs
 public static event System.Action<string> onExport;
```

Your save function should have a string parameter to receive the formatted file data.

## Custom Exporters

The asset supports easy custom exporters for the answers given by the user.

* Create a new class for your exporter

*  Implement the `IQuestionnaireExporter` interface, your class should now automatically appear via dropdown in the `Export Settings` group.

*  For other file types other than `txt` the Extension property can be used
```cs
public string Extension => ".csv";
```
* `IAnswerElement` can be extended to record more information, adding a new field should be easy enough as it will mark every script that currently implements the interface.

## Settings

### Localization

## Issues

 * There are no known issues at the moment

[title]: images/title.png
[exportSettings]: images/exportSettings.png
[githubProfile]: (https://github.com/RodrigoPrinheiro)