using System.Reflection;

namespace NetDocGen
{
	public class EventDocumentation : MemberDocumentation<EventInfo, TypeDocumentation>
	{
		public EventDocumentation(string fullName) : base(fullName) { }

		public EventDocumentation(EventInfo eventInfo) : base(eventInfo)
		{
		}

		public EventDocumentation(EventInfo eventInfo, TypeDocumentation owner) : base(eventInfo, owner)
		{
		}
	}
}
