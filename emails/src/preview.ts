import { render } from '@react-email/render';
import { CreditEmail } from './CreditEmail.js';

const html = await render(
  CreditEmail({
    clientName: 'Pepito Perez',
    clientId: '1001',
    amount: 7800000,
    interestRate: 2,
    termMonths: 10,
    commercialName: 'Ana Comercial',
    registeredAtUtc: new Date().toISOString(),
  }),
);

console.log(html);
