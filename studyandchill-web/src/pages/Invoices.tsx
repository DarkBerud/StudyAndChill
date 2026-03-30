import { useState, useEffect } from 'react';
import { Box, Typography, Paper, useTheme, Chip, Button, CircularProgress } from '@mui/material';
import Grid from '@mui/material/Grid';
import ReceiptIcon from '@mui/icons-material/Receipt';
import EventIcon from '@mui/icons-material/Event';
import PaymentIcon from '@mui/icons-material/Payment';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';

interface Invoice {
    id: string;
    description: string;
    dueDate: string;
    amount: number;
    status: 'Paga' | 'Pendente' | 'Atrasada';
    paymentLink?: string;
}

export default function Invoices() {
    const theme = useTheme();
    const navigate = useNavigate();

    const [invoices, setInvoices] = useState<Invoice[]>([]);
    const [loading, setLoading] = useState(true);

    const getStatusConfig = (status: string) => {
        switch (status) {
            case 'Paga': return { chipColor: 'success' as const, label: 'Paga', borderColor: theme.palette.success.main };
            case 'Pendente': return { chipColor: 'warning' as const, label: 'Pendente', borderColor: theme.palette.warning.main };
            case 'Atrasada': return { chipColor: 'error' as const, label: 'Atrasada', borderColor: theme.palette.error.main };
            default: return { chipColor: 'default' as const, label: status, borderColor: theme.palette.divider };
        }
    };

    const formatCurrency = (value: number) => {
        return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);
    };

    useEffect(() => {
        const fetchInvoices = async () => {
            try {
                const response = await api.get('/financial/student/invoices');

                setInvoices(response.data);

            } catch (error) {
                console.error('Erro ao buscar faturas', error);
            } finally {
                setLoading(false);
            }
        };

        fetchInvoices();
    }, []);

    return (
        <Box>
            <Box sx={{ display: 'flex', gap: 3, alignItems: 'center', mb: 4 }}>
                <Typography variant='h4' sx={{ fontFamily: 'Bodoni Moda', color: theme.palette.primary.main, fontWeight: 'bold' }}>
                    Minhas Faturas
                </Typography>
                <Button variant='outlined' onClick={() => navigate('/dashboard')} sx={{ borderRadius: 3 }}>
                    Voltar
                </Button>
            </Box>

            {loading ? (
                <Box sx={{ display: 'flex', justifyContent: 'center', mt: 10 }}>
                    <CircularProgress />
                </Box>
            ) : invoices.length === 0 ? (
                <Paper elevation={0} sx={{ p: 4, textAlign: 'center', bgcolor: 'transparent', border: `1px dashed ${theme.palette.divider}` }}>
                    <ReceiptIcon sx={{ fontSize: 48, color: theme.palette.text.disabled, mb: 2 }} />
                    <Typography variant='h6' color="texteSecondary">Nenhuma fatura encontrada</Typography>
                </Paper>
            ) : (
                <Grid container spacing={3}>
                    {invoices.map((invoice) => {
                        const statusConfig = getStatusConfig(invoice.status);
                        const formatedDate = new Date(invoice.dueDate).toLocaleDateString('pt-BR', { timeZone: 'UTC' });

                        return (
                            <Grid size={{ xs: 12, md: 6, lg: 4 }} key={invoice.id}>
                                <Paper
                                    elevation={0}
                                    sx={{
                                        p: 3,
                                        borderRadius: theme.shape.borderRadius,
                                        border: `1px solid ${theme.palette.divider}`,
                                        borderLeft: `6px solid ${statusConfig.borderColor}`,
                                        display: 'flex',
                                        flexDirection: 'column',
                                        gap: 2,
                                        transition: 'box-shadow 0.2s',
                                        '&:hover': { boxShadow: 2 }
                                    }}
                                >
                                    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                                        <Typography variant='h6' sx={{ fontWeight: 'bold', color: theme.palette.text.primary, lineHeight: 1.2 }}>
                                            {invoice.description}
                                        </Typography>
                                        <Chip
                                            label={statusConfig.label}
                                            size='small'
                                            color={statusConfig.chipColor}
                                            sx={{ fontWeight: 'bold' }}
                                        />
                                    </Box>

                                    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1, mt: 1 }}>
                                        <Box>
                                            <EventIcon fontSize='small' />
                                            <Typography variant='body2'>Vencimento: {formatedDate}</Typography>
                                        </Box>
                                        <Typography variant="h5" sx={{ fontWeight: 'bold', color: theme.palette.primary.main, mt: 1 }}>
                                            {formatCurrency(invoice.amount)}
                                        </Typography>
                                    </Box>

                                    {invoice.status !== 'Paga' && (
                                        <Button
                                            variant='contained'
                                            color='primary'
                                            fullWidth
                                            startIcon={<PaymentIcon />}
                                            sx={{ mt: 1, borderRadius: 2 }}
                                            onClick={() => invoice.paymentLink ? window.open(invoice.paymentLink, '_blank') : alert('Link não disponível')}
                                        >
                                            Pagar Agora
                                        </Button>
                                    )}

                                </Paper>
                            </Grid>
                        );
                    })}
                </Grid>
            )}
        </Box>
    );

}