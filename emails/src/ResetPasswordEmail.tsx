import {
  Body,
  Button,
  Container,
  Head,
  Heading,
  Html,
  Preview,
  Section,
  Text,
} from '@react-email/components';

export interface ResetPasswordEmailProps {
  email: string;
  resetUrl: string;
}

const brand = '#00d280';
const dark = '#052224';

export function ResetPasswordEmail(props: ResetPasswordEmailProps) {
  return (
    <Html>
      <Head />
      <Preview>Restablece tu contraseña en Fya Credits</Preview>
      <Body style={body}>
        <Container style={container}>
          <Section style={header}>
            <Text style={brandLabel}>Fya Social Capital</Text>
            <Heading style={title}>Restablece tu contraseña</Heading>
          </Section>

          <Section style={content}>
            <Text style={text}>
              Recibimos una solicitud para restablecer la contraseña de{' '}
              <strong>{props.email}</strong>.
            </Text>
            <Text style={text}>
              Haz clic en el botón para elegir una nueva contraseña. Este enlace
              vence en unas horas.
            </Text>
            <Button href={props.resetUrl} style={button}>
              Restablecer contraseña
            </Button>
            <Text style={muted}>
              Si no solicitaste este cambio, ignora este correo y tu contraseña
              seguirá igual.
            </Text>
          </Section>

          <Section style={footer}>
            <Text style={footerText}>Enviado automáticamente por Fya Credits.</Text>
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

const text: React.CSSProperties = {
  color: '#374151',
  fontSize: 15,
  lineHeight: 1.6,
  margin: '0 0 16px',
};

const button: React.CSSProperties = {
  backgroundColor: brand,
  color: dark,
  fontWeight: 700,
  padding: '12px 20px',
  borderRadius: 10,
  textDecoration: 'none',
  display: 'inline-block',
};

const muted: React.CSSProperties = {
  color: '#6b7280',
  fontSize: 13,
  lineHeight: 1.5,
  margin: '20px 0 0',
};

const footer: React.CSSProperties = {
  backgroundColor: '#f3f4f6',
  padding: '20px 32px',
  textAlign: 'center' as const,
};

const footerText: React.CSSProperties = {
  color: '#6b7280',
  fontSize: 12,
  margin: 0,
};
