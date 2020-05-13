using Data;
using Logic;
using Presentation.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Presentation.ViewModel
{
    public class DialogOrderEditViewModel : DialogDataEditViewModel<Order>
    {
        private uint _Id;
        private string[] _ClientUsernames;
        private Dictionary<uint, uint> _ProductIdQuantityMap;
        private int _ClientUsernameIndex;
        private DateTime _OrderDate, _DeliveryDate;
        private double _Price;
        private bool _Delivered;

        public DialogOrderEditViewModel(IDialogHost dialogHost, IDataRepository dataRepository) : base(dialogHost, dataRepository) { }

        public int ClientUsernameIndex
        {
            get
            {
                return _ClientUsernameIndex;
            }
            set
            {
                _ClientUsernameIndex = value;
                RaisePropertyChanged();
            }
        }

        public bool Delivered
        {
            get
            {
                return _Delivered;
            }
            set
            {
                _Delivered = value;
                RaisePropertyChanged();
            }
        }

        public DateTime OrderDate
        {
            get
            {
                return _OrderDate;
            }
            set
            {
                _OrderDate = value;
                RaisePropertyChanged();
            }
        }

        public DateTime DeliveryDate
        {
            get
            {
                return _DeliveryDate;
            }
            set
            {
                _DeliveryDate = value;
                RaisePropertyChanged();
            }
        }

        public string[] ClientUsernames
        {
            get
            {
                return _ClientUsernames;
            }
            set
            {
                _ClientUsernames = value;
                RaisePropertyChanged();
            }
        }

        protected override bool CanApply()
        {
            // TODO: add ProductIdQuantityMap GUI configuration and adjust CanApply here
            return ClientUsernameIndex != -1 && (!Delivered || DeliveryDate >= OrderDate);
        }

        protected override void ApplyCreate()
        {
            throw new NotImplementedException();
        }

        protected override void ApplyEdit()
        {
            DataRepository.Update(new Order(_Id, ClientUsernames[ClientUsernameIndex], OrderDate, _ProductIdQuantityMap, _Price, Delivered ? DeliveryDate : (DateTime?)null));
        }

        protected override void InjectProperties(Order toUpdate)
        {
            UpdateClientUsernames();
            _Id = toUpdate.Id;
            _ProductIdQuantityMap = toUpdate.ProductIdQuantityMap;
            OrderDate = toUpdate.OrderDate;
            _Price = toUpdate.Price;
            if (toUpdate.DeliveryDate.HasValue)
            {
                DeliveryDate = toUpdate.DeliveryDate.Value;
                Delivered = true;
            }
            else
            {
                DeliveryDate = DateTime.Now;
                Delivered = false;
            }
            ClientUsernameIndex = -1;
            for (int i = 0; i < ClientUsernames.Length; ++i)
            {
                if (ClientUsernames[i] == toUpdate.ClientUsername)
                {
                    ClientUsernameIndex = i;
                    break;
                }
            }
        }

        protected override void ResetProperties()
        {
            UpdateClientUsernames();
            OrderDate = DateTime.Now;
            DeliveryDate = DateTime.Now;
            Delivered = false;
            ClientUsernameIndex = 0;
            throw new NotImplementedException();
        }

        private void UpdateClientUsernames()
        {
            ClientUsernames = DataRepository.GetAllClients().Select(c => c.Username).ToArray();
        }
    }
}
