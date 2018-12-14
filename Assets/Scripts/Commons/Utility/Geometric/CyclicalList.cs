using System.Collections.Generic;

namespace nopact.Commons.Utility.Geometric
{
	class CyclicList<T> : List<T>
	{
		public new T this[int index]
		{
			get
			{

				while (index < 0 )
                {

                    index = Count + index;

                }
					
				if (index >= Count)
                {

                    index %= Count;

                }					

				return base[index];
            }
			set
			{
			
				while (index < 0 )
                {

                    index = Count + index;

                }
					
				if (index >= Count)
                {

                    index %= Count;

                }
					

				base[index] = value;

			}

		}

		public CyclicList() { }

		public CyclicList(IEnumerable<T> collection) : base(collection)
		{
		}

		public new void RemoveAt(int index)
		{

			Remove(this[index]);

		}

	}

}
