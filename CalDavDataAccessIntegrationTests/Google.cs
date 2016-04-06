// This file is Part of CalDavSynchronizer (http://outlookcaldavsynchronizer.sourceforge.net/)
// Copyright (c) 2015 Gerhard Zehetbauer
// Copyright (c) 2015 Alexander Nimmervoll
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Threading.Tasks;
using CalDavSynchronizer.Contracts;
using NUnit.Framework;

namespace CalDavDataAccessIntegrationTests
{
  public class Google : FixtureBase
  {
    protected override string ProfileName
    {
      get { return "Google-TestCal"; }
    }

    protected override ServerAdapterType? ServerAdapterTypeOverride
    {
      get { return ServerAdapterType.WebDavHttpClientBasedWithGoogleOAuth; }
    }

    [Ignore ("Google doesnt create a new entity in that case, it fails with precondition.")]
    public override async System.Threading.Tasks.Task UpdateNonExistingEntity_CreatesNewEntity ()
    {
      await base.UpdateNonExistingEntity_CreatesNewEntity();
    }

    [Ignore ("Google doesnt fails with preconditions on DELETE.")]
    public override async System.Threading.Tasks.Task DeleteEntityWithWrongVersion_PreconditionFails ()
    {
      await base.DeleteEntityWithWrongVersion_PreconditionFails();
    }

    [Ignore ("Google doesnt fails with preconditions on DELETE.")]
    public override async System.Threading.Tasks.Task DeleteNonExistingEntity_PreconditionFails ()
    {
      await base.DeleteNonExistingEntity_PreconditionFails();
    }

    [Test]
    public override async Task Test_CRUD ()
    {
      await base.Test_CRUD ();
    }
  }
}