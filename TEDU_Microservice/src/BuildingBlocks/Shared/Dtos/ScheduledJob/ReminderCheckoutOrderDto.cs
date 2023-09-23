using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.ScheduledJob;
public record ReminderCheckoutOrderDto(string email, string subject, string emailContent, DateTimeOffset enqueueAt);
