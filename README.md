# processamento-pedidos-rabbitmq
Projeto de estudo destinado para criar um modelo de processamento de pedidos utilizando mensageria(RabbitMQ):

Estrutura atual:

1 API Pedidos simulando o serviço que irá receber a solicitação de um novo pedido (Irá jogar o pedido na fila de pagamento);
1 biblioteca para centralizar o uso do Rabbit (pontos de melhorias a serem realizados futuramente);
3 Workers:
    Worker para processar o pagamento
    Worker para separação em estoque e emissão de nota fiscal
    Worker para envio do pedido para transportadora
    
