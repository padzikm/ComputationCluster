﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ten kod został wygenerowany przez narzędzie.
//     Wersja wykonawcza:4.0.30319.34011
//
//     Zmiany w tym pliku mogą spowodować nieprawidłowe zachowanie i zostaną utracone, jeśli
//     kod zostanie ponownie wygenerowany.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// This source code was auto-generated by xsd, Version=4.0.30319.18020.
// 


/// <uwagi/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.18020")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.mini.pw.edu.pl/ucc/")]
[System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.mini.pw.edu.pl/ucc/", IsNullable=false)]
public partial class SolveRequest {
    
    private string problemTypeField;
    
    private string solvingTimeoutField;
    
    private byte[] dataField;
    
    /// <uwagi/>
    public string ProblemType {
        get {
            return this.problemTypeField;
        }
        set {
            this.problemTypeField = value;
        }
    }
    
    /// <uwagi/>
    public string SolvingTimeout {
        get {
            return this.solvingTimeoutField;
        }
        set {
            this.solvingTimeoutField = value;
        }
    }
    
    /// <uwagi/>
    [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
    public byte[] Data {
        get {
            return this.dataField;
        }
        set {
            this.dataField = value;
        }
    }
}
