using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDocGen.Services
{
	public interface IMarkDownGenerator
	{
		void Generate(AssemblyDocumentation documentation);
	}
}
