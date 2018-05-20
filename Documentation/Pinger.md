# Pinger
Small project containing two parts: a client installed on the server we need to test and get metrics about, and a windows service server installed on a central server that contains ElasticSearch, Kibana and/or Grafana for visualization.

## Motivation
Basically I needed some pretty simple metric about how the server acted, and I needed it without paying a high price in the sense of money. A lot of the good tools offer a lot of functionality that I don't need or use, so instead of buying a took to get my metric, I build a lightweight client.

## Client
The big part of this is the client. The client reads two configuration files:


## Server


## Visualization

![Grafana visualization of the data](./dumps/2018-05-20_2347.png)