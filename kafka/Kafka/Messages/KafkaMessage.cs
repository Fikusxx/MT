﻿namespace Kafka.Messages;

public sealed record KafkaMessage(Guid Id, string Value);

public sealed record KafkaMessageError;