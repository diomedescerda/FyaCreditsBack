import {
  Body,
  Column,
  Container,
  Head,
  Heading,
  Hr,
  Html,
  Preview,
  Row,
  Section,
  Text,
} from '@react-email/components';

export interface CreditEmailProps {
  clientName: string;
  clientId: string;
  amount: number;
  interestRate: number;
  termMonths: number;
  commercialName: string;
  registeredAtUtc: string;
}

const brand = '#00d280';
const dark = '#052224';
const bodyText = '#374151';
const muted = '#6b7280';

const currency = new Intl.NumberFormat('es-CO', {
  style: 'currency',
  currency: 'COP',
  maximumFractionDigits: 0,
});

const numberFormat = new Intl.NumberFormat('es-CO', { maximumFractionDigits: 2 });

export function CreditEmail(props: CreditEmailProps) {
  const amountFormatted = currency.format(props.amount);
  const rateFormatted = `${numberFormat.format(props.interestRate)}%`;
  const dateFormatted = new Intl.DateTimeFormat('es-CO', {
    dateStyle: 'long',
    timeStyle: 'short',
  }).format(new Date(props.registeredAtUtc));

  return (
    <Html>
      <Head />
      <Preview>Nuevo crédito registrado para {props.clientName}</Preview>
      <Body style={body}>
        <Container style={container}>
          <Section style={header}>
            <Text style={brandLabel}>Fya Social Capital</Text>
            <Heading style={title}>Nuevo crédito registrado</Heading>
          </Section>

          <Section style={content}>
            <Text style={label}>Cliente</Text>
            <Text style={value}>{props.clientName}</Text>

            <Text style={label}>Cédula o ID</Text>
            <Text style={value}>{props.clientId}</Text>

            <Hr style={hr} />

            <Text style={label}>Valor del crédito</Text>
            <Text style={amount}>{amountFormatted}</Text>

            <Hr style={hr} />

            <Section style={summary}>
              <Row>
                <Column style={cell}>
                  <Text style={cellLabel}>Plazo</Text>
                  <Text style={cellValue}>{props.termMonths} meses</Text>
                </Column>
                <Column style={cell}>
                  <Text style={cellLabel}>Tasa de interés (NM)</Text>
                  <Text style={cellValue}>{rateFormatted}</Text>
                </Column>
              </Row>
              <Row>
                <Column style={cell}>
                  <Text style={cellLabel}>Comercial</Text>
                  <Text style={cellValue}>{props.commercialName}</Text>
                </Column>
                <Column style={cell}>
                  <Text style={cellLabel}>Fecha de registro</Text>
                  <Text style={cellValue}>{dateFormatted}</Text>
                </Column>
              </Row>
            </Section>
          </Section>

          <Section style={footer}>
            <Text style={footerText}>Registrado automáticamente por la plataforma Fya Credits.</Text>
          </Section>
        </Container>
      </Body>
    </Html>
  );
}

const body: React.CSSProperties = {
  margin: 0,
  padding: '0',
  backgroundColor: '#eef2f1',
  fontFamily:
    "-apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif",
};

const container: React.CSSProperties = {
  maxWidth: 560,
  margin: '0 auto',
  backgroundColor: '#ffffff',
  borderRadius: 18,
  overflow: 'hidden',
  border: '1px solid rgba(0, 210, 128, 0.25)',
};

const header: React.CSSProperties = {
  backgroundColor: dark,
  padding: '28px 32px',
};

const brandLabel: React.CSSProperties = {
  color: brand,
  fontSize: 12,
  fontWeight: 700,
  letterSpacing: 2,
  textTransform: 'uppercase',
  margin: 0,
};

const title: React.CSSProperties = {
  color: '#ffffff',
  fontSize: 22,
  lineHeight: 1.2,
  margin: '8px 0 0',
};

const content: React.CSSProperties = {
  padding: '28px 32px',
};

const label: React.CSSProperties = {
  color: muted,
  fontSize: 12,
  fontWeight: 700,
  textTransform: 'uppercase',
  letterSpacing: 1,
  margin: '0 0 4px',
};

const value: React.CSSProperties = {
  color: bodyText,
  fontSize: 16,
  fontWeight: 600,
  margin: '0 0 16px',
};

const amount: React.CSSProperties = {
  color: dark,
  fontSize: 28,
  fontWeight: 800,
  margin: 0,
};

const hr: React.CSSProperties = {
  borderColor: '#e5e7eb',
  margin: '16px 0',
};

const summary: React.CSSProperties = {
  backgroundColor: '#f7f9f8',
  borderRadius: 12,
  padding: '16px 20px',
};

const cell: React.CSSProperties = {
  width: '50%',
  padding: '4px 0',
  verticalAlign: 'top',
};

const cellLabel: React.CSSProperties = {
  color: muted,
  fontSize: 11,
  fontWeight: 700,
  textTransform: 'uppercase',
  letterSpacing: 1,
  margin: '0 0 2px',
};

const cellValue: React.CSSProperties = {
  color: bodyText,
  fontSize: 14,
  fontWeight: 600,
  margin: 0,
};

const footer: React.CSSProperties = {
  backgroundColor: '#f3f4f6',
  padding: '20px 32px',
  textAlign: 'center' as const,
};

const footerText: React.CSSProperties = {
  color: muted,
  fontSize: 12,
  margin: 0,
};
