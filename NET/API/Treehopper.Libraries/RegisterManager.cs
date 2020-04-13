using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries
{
    public class RegisterManager
    {
        protected List<Register> _registers = new List<Register>();
        private IRegisterManagerAdapter adapter;
        protected bool multiRegisterAccess;

        public RegisterManager(IRegisterManagerAdapter adapter, bool multiRegisterAccess)
        {
            this.adapter = adapter;
            this.multiRegisterAccess = multiRegisterAccess;
        }

        public async Task read(Register register)
        {
            var data = await adapter.read(register.Address, register.Width).ConfigureAwait(false);
            register.setBytes(data);
        }

        public async Task readRange(Register start, Register end)
        {
            if (multiRegisterAccess)
            {
                var count = (end.Address + end.Width) - start.Address;
                var bytes = await adapter.read(start.Address, count).ConfigureAwait(false);
                int i = 0;
                foreach (var reg in _registers.Where(reg => reg.Address >= start.Address && reg.Address <= end.Address))
                {
                    reg.setBytes(bytes.Skip(i).Take(reg.Width).ToArray());
                    i += reg.Width;
                }
            }
            else
            {
                foreach (var reg in _registers.Where(reg => reg.Address >= start.Address && reg.Address <= end.Address))
                {
                    await read(reg);
                }
            }
        }

        public Task write(Register register)
        {
            return adapter.write(register.Address, register.getBytes());
        }

        public async Task writeRange(Register start, Register end)
        {
            if (multiRegisterAccess)
            {
                List<byte> bytes = new List<byte>();
                foreach (var reg in _registers.Where(reg => reg.Address >= start.Address && reg.Address <= end.Address))
                {
                    bytes.AddRange(reg.getBytes());
                }
                await adapter.write(start.Address, bytes.ToArray()).ConfigureAwait(false);
            }
            else
            {
                foreach (var reg in _registers.Where(reg => reg.Address >= start.Address && reg.Address <= end.Address))
                {
                    await write(reg).ConfigureAwait(false);
                }
            }
        }
    }
}
