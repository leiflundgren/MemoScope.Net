﻿using BrightIdeasSoftware;
using MemoScope.Core;
using MemoScope.Core.Helpers;
using Microsoft.Diagnostics.Runtime;
using System.Collections.Generic;
using System.Linq;
using MemoScope.Core.Data;

namespace MemoScope.Modules.TypeDetails
{
    public partial class TypeDetailsModule : UIClrDumpModule
    {
        private ClrType type;
        public TypeDetailsModule()
        {
            InitializeComponent();
            Icon = Properties.Resources.blueprint;
        }

        public void Setup(ClrDumpType dumpType)
        {
            type = dumpType.ClrType;
            ClrDump = dumpType.ClrDump;
            pgTypeInfo.SelectedObject = new TypeInformations(dumpType);

            Generator.GenerateColumns(dlvFields, typeof(FieldInformation), false);
            dlvFields.SetUpTypeColumn(nameof(FieldInformation.Type));
            dlvFields.SetObjects(dumpType.Fields.Select(clrField => new FieldInformation(dumpType, clrField)));
            dlvFields.RegisterDataProvider(() => {
                return new ClrDumpType(ClrDump, dlvFields.SelectedObject<FieldInformation>()?.ClrType);
            }, this);

            Generator.GenerateColumns(dlvMethods, typeof(MethodInformation), false);
            dlvMethods.SetUpTypeColumn(nameof(MethodInformation.Type));
            dlvMethods.SetObjects(dumpType.Methods.Select(clrMethod => new MethodInformation(dumpType, clrMethod)));
            dlvMethods.RegisterDataProvider(() => {
                return new ClrDumpType(ClrDump, dlvMethods.SelectedObject<MethodInformation>()?.ClrType);
            }, this);

            dtlvParentClasses.InitData<AbstractTypeInformation>();
            dtlvParentClasses.SetUpTypeColumn(nameof(AbstractTypeInformation.Name));

            var l = new List<object>();
            var typeInformation = new TypeInformation(dumpType.BaseType);
            var interfaceInformations = InterfaceInformation.GetInterfaces(dumpType);
            l.Add(typeInformation);
            l.AddRange(interfaceInformations);
            dtlvParentClasses.Roots = l;
        }

        public override void PostInit()
        {
            Name = $"#{ClrDump.Id} - {type.Name}";
            Summary = type.Name;
        }
    }
}
