/*******************************************************************************
 * MIT License
 *
 * Copyright 2021 Provision Data Systems Inc.  https://provisiondata.com
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sub-license,
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 *
 *******************************************************************************/

namespace ProvisionData.Specifications;

public record Person(Guid Id, String Name, DateTime DateOfBirth, Gender Gender);

[Flags]
public enum Gender
{
	Unknown = 0,
	Female = 1,
	Male = 2,
	Other = 4
}

public class UserHasGender : Query<Person>
{
	public UserHasGender(Gender gender) : base(user => (gender & user.Gender) != 0) { }
}

public class UserIsAgeOfMajority : Query<Person>
{
	public UserIsAgeOfMajority(Int32 ageOfMajority)
		: this(DateTime.UtcNow.Date.AddYears(0 - ageOfMajority))
	{
	}

	public UserIsAgeOfMajority(DateTime dateTime) : base(user => user.DateOfBirth <= dateTime) { }
}
