﻿/*
    Copyright (C) 2014-2016 de4dot@gmail.com

    This file is part of dnSpy

    dnSpy is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    dnSpy is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with dnSpy.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using dnlib.PE;
using dnSpy.Contracts.Files;
using dnSpy.Contracts.Files.TreeView;
using dnSpy.Contracts.Images;
using dnSpy.Contracts.Languages;
using dnSpy.Contracts.Text;
using dnSpy.Contracts.TreeView;
using dnSpy.Languages;

namespace dnSpy.Files.TreeView {
	sealed class PEFileNode : DnSpyFileNode, IPEFileNode {
		public PEFileNode(IDnSpyFile dnSpyFile)
			: base(dnSpyFile) {
			Debug.Assert(dnSpyFile.PEImage != null && dnSpyFile.ModuleDef == null);
		}

		public override Guid Guid => new Guid(FileTVConstants.PEFILE_NODE_GUID);
		protected override ImageReference GetIcon(IDotNetImageManager dnImgMgr) => dnImgMgr.GetImageReference(DnSpyFile.PEImage);
		public bool IsExe => (DnSpyFile.PEImage.ImageNTHeaders.FileHeader.Characteristics & Characteristics.Dll) == 0;
		public override void Initialize() => TreeNode.LazyLoading = true;

		public override IEnumerable<ITreeNodeData> CreateChildren() {
			foreach (var file in DnSpyFile.Children)
				yield return Context.FileTreeView.CreateNode(this, file);
		}

		protected override void Write(IOutputColorWriter output, ILanguage language) =>
			new NodePrinter().Write(output, language, DnSpyFile);

		protected override void WriteToolTip(IOutputColorWriter output, ILanguage language) {
			output.Write(BoxedOutputColor.EnumField, TargetFrameworkUtils.GetArchString(DnSpyFile.PEImage.ImageNTHeaders.FileHeader.Machine));

			output.WriteLine();
			output.WriteFilename(DnSpyFile.Filename);
		}

		public override FilterType GetFilterType(IFileTreeNodeFilter filter) =>
			filter.GetResult(DnSpyFile).FilterType;
	}
}
