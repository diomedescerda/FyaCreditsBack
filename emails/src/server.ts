import express from 'express';
import { render } from '@react-email/render';
import { CreditEmail, type CreditEmailProps } from './CreditEmail.js';

const app = express();
app.use(express.json({ limit: '128kb' }));

app.post('/render', async (req, res) => {
  const props = req.body as Partial<CreditEmailProps>;

  if (!props.clientName || !props.clientId || typeof props.amount !== 'number') {
    res.status(400).json({ error: 'clientName, clientId and amount are required.' });
    return;
  }

  const html = await render(
    CreditEmail({
      clientName: props.clientName,
      clientId: props.clientId,
      amount: props.amount,
      interestRate: props.interestRate ?? 0,
      termMonths: props.termMonths ?? 0,
      commercialName: props.commercialName ?? '',
      registeredAtUtc: props.registeredAtUtc ?? new Date().toISOString(),
    }),
  );

  console.log(`Rendered credit notification for ${props.clientName}`);
  res.type('html').send(html);
});

app.get('/preview', async (_req, res) => {
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
  res.type('html').send(html);
});

app.get('/health', (_req, res) => {
  res.json({ status: 'ok' });
});

const port = Number(process.env.PORT ?? 3000);
app.listen(port, () => {
  console.log(`Email renderer listening on http://localhost:${port}`);
});
