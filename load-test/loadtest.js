import ws from 'k6/ws';
import { check, sleep } from 'k6';
import { SharedArray } from 'k6/data';
import { uuidv4 } from 'https://jslib.k6.io/k6-utils/1.1.0/index.js';

export const options = {
  stages: [
    { duration: '1m', target: 200 }, // 1 分鐘內將虛擬用戶數增加到 100
    { duration: '3m', target: 200 }, // 維持 3 分鐘的負載
    { duration: '2m', target: 0 },   // 2 分鐘內將虛擬用戶數降至 0
  ],
};

export default function () {
  const url = 'ws://localhost:5001/hub';

  const res = ws.connect(url, {}, function (socket) {
    let messageCount = 10; // 發送消息的總數
    let receivedCount = 0; // 已收到回應的計數器

    socket.on('open', function () {
      console.log('Connected');

      // 初始化 SignalR 連接
      const negotiationMessage = {
        protocol: 'json',
        version: 1,
      };
      socket.send(JSON.stringify(negotiationMessage) + '\x1E');
      
      // 發送多個 'SendRequest' 事件
      for (let i = 0; i < messageCount; i++) {
        const requestId = uuidv4();
        const requestData = { message: `Hello from VU ${__VU}, iteration ${i}` };

        const invocationMessage = {
          type: 1,
          target: 'SendRequest',
          arguments: [requestId, requestData],
        };
        socket.send(JSON.stringify(invocationMessage) + '\x1E');

      }
    });

    socket.on('message', function (data) {
      // 處理服務器返回的消息
      const messages = data.split('\x1E');
      messages.forEach((message) => {
        if (message) {
          const msg = JSON.parse(message);
          console.log(`Received message: ${JSON.stringify(msg)}`);

          // 根據您的服務器返回的消息結構，判斷是否是對 'SendRequest' 的回應
          if (msg.type === 1 && msg.target === 'ReceiveResponse') {
            receivedCount++;
          }
        }
      });

      // 如果已收到所有回應，關閉連接
      if (receivedCount >= messageCount) {
        console.log('All messages received, closing socket');
        socket.close();
      }
    });

    socket.on('close', function () {
      console.log('Disconnected');
    });

    // 移除自動關閉的 timeout，以避免連接過早關閉
    // socket.setTimeout(function () {
    //   console.log('Closing the socket due to timeout');
    //   socket.close();
    // }, 60000); // 可選，若需要設定最大連接時間
  });

  check(res, { 'status is 101': (r) => r && r.status === 101 });
}