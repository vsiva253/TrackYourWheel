using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackYourWheelz.EntityFrameworkCore.Context
{
    public class TrackYourWheelzDbContext : DbContext
    {
        public TrackYourWheelzDbContext(DbContextOptions<TrackYourWheelzDbContext> options) : base(options)
        {

        }
    }
}
