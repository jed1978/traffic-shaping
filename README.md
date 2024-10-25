# Traffic Shaping with SignalR and Redis in ASP.NET Core

# Overview

This repository contains a sample implementation of a traffic shaping mechanism using SignalR and Redis in an ASP.NET Core application. The solution demonstrates how to handle high levels of concurrent requests by buffering incoming traffic, thus smoothing out sudden spikes and preventing server overload.

## 1. RequestController (ASP.NET Core Web Application)
- **Description**: Acts as the front-end server that accepts client connections using SignalR (WebSocket). It collects incoming requests from clients, assigns a unique Request ID, and enqueues them into a Redis queue for processing.
- **Responsibilities**:
  - Manage client connections via SignalR Hub.
  - Enqueue incoming requests into Redis.
  - Subscribe to Redis channels to receive processing results.
  - Send processing results back to the corresponding clients.

## 2. WorkerService (.NET Core Console Application)
- **Description**: Serves as the back-end processor that dequeues requests from Redis, processes them, and publishes the results back to a Redis channel.
- **Responsibilities**:
  - Dequeue requests from the Redis queue.
  - Perform the necessary processing on each request.
  - Publish the processing results to a Redis channel for the RequestController to consume.

## Features
- **Real-time Communication**: Utilizes SignalR for real-time, bi-directional communication between the server and clients.
- **Traffic Shaping**: Implements a buffering mechanism using Redis queues to handle high traffic volumes smoothly.
- **Scalability**: Supports horizontal scaling by running multiple instances of the WorkerService, enabling higher throughput.
- **Asynchronous Processing**: Decouples request reception from processing, allowing the system to handle requests asynchronously without blocking client connections.
- **Latest Technologies**: Built using the latest versions of .NET and SignalR, ensuring compatibility and leveraging new features.

	Real-time Communication: Utilizes SignalR for real-time, bi-directional communication between the server and clients.
	Traffic Shaping: Implements a buffering mechanism using Redis queues to handle high traffic volumes smoothly.
	Scalability: Supports horizontal scaling by running multiple instances of the WorkerService, enabling higher throughput.
	Asynchronous Processing: Decouples request reception from processing, allowing the system to handle requests asynchronously without blocking client connections.
	Latest Technologies: Built using the latest versions of .NET and SignalR, ensuring compatibility and leveraging new features.
