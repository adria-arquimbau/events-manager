﻿using EventsManager.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventsManager.Server.Data.TypeConfigurations;

public class EventTypeConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Owner)
            .WithMany(o => o.OwnedEvents)
            .HasForeignKey(x => x.OwnerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}