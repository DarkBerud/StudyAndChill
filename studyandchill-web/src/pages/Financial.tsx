import { useState, useEffect } from 'react';
import { Box, Typography, Paper, useTheme, Chip, Button, CircularProgress, Divider} from '@mui/material';
import Grid from '@mui/material/Grid';
import AttachMoneyIcon from '@mui/icons-material/AttachMoney';
import AccountBalanceWalletIcon from '@mui/icons-material/AccountBalanceWallet';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import PendingIcon from '@mui/icons-material/Pending';
import ErrorOutlineIcon from '@mui/icons-material/ErrorOutline';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';

interface Commission {
    id: number;
    studentName: string;
    contractId: number;
    dueDate: string;
    commissionAmount: number;
    status: 'Pendente' | 'Recebido' | 'Atrasado';
}

export default function Financial() {
    const theme = useTheme();
    const navigate = useNavigate();

    const [commissions, setCommissions] = useState<Commission[]>([]);
    const [loading, setLoading] = useState(true);

    const formatCurrency = (value: number) => {
        return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);
    };

    const getStatusConfig = (status: string) => {
        switch (status) {
            case 'Recebido': return { color: 'success' as const, icon: <CheckCircleIcon fontSize='small' /> };
            case 'Pendente': return { color: 'warning' as const, icon: <PendingIcon fontSize='small' /> };
            case 'Atrasado': return { color: 'error' as const, icon: <ErrorOutlineIcon fontSize="small" /> };
            default: return { color: 'default' as const, icon: undefined };
        }
    };

    useEffect(() => {
    const fetchFinancialData = async () => {
      try {
        // Agora bate no C# de verdade!
        const response = await api.get('/financial/teacher/commissions');
        
        // Coloca os dados da API no estado da tela
        setCommissions(response.data);

      } catch (error) {
        console.error('Erro ao buscar financeiro:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchFinancialData();
  }, []);
    const totalReceived = commissions.filter(c => c.status === 'Recebido').reduce((acc, curr) => acc + curr.commissionAmount, 0);
    const totalPending = commissions.filter(c => c.status === 'Pendente' || c.status === 'Atrasado').reduce((acc, curr) => acc + curr.commissionAmount, 0);

    return (
        <Box>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 3, mb: 4 }}>
                <Typography variant="h4" sx={{ fontFamily: 'Bodoni Moda', color: theme.palette.primary.main, fontWeight: 'bold' }}>
                    Minhas Comissões
                </Typography>
                <Button variant="outlined" onClick={() => navigate('/dashboard')} sx={{ borderRadius: 3, size: 'small' }}>
                    Voltar
                </Button>
            </Box>

            {loading ? (
                <Box sx={{ display: 'flex', justifyContent: 'center', mt: 10 }}>
                    <CircularProgress />
                </Box>
            ) : (
                <>
                    <Grid container spacing={3} sx={{ mb: 4 }}>
                        <Grid size={{ xs: 12, sm: 6 }}>
                            <Paper elevation={0} sx={{ p: 3, borderRadius: 3, bgcolor: theme.palette.success.light, color: theme.palette.success.dark, display: 'flex', alignItems: 'center', gap: 2 }}>
                                <AccountBalanceWalletIcon sx={{ fontSize: 48, opacity: 0.8 }} />
                                <Box>
                                    <Typography variant="body1" sx={{ fontWeight: 'bold', opacity: 0.8 }}>Total Recebido</Typography>
                                    <Typography variant="h4" sx={{ fontWeight: 'bold' }}>{formatCurrency(totalReceived)}</Typography>
                                </Box>
                            </Paper>
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6 }}>
                            <Paper elevation={0} sx={{ p: 3, borderRadius: 3, bgcolor: theme.palette.warning.light, color: theme.palette.warning.dark, display: 'flex', alignItems: 'center', gap: 2 }}>
                                <AttachMoneyIcon sx={{ fontSize: 48, opacity: 0.8 }} />
                                <Box>
                                    <Typography variant="body1" sx={{ fontWeight: 'bold', opacity: 0.8 }}>A Receber (Pendente)</Typography>
                                    <Typography variant="h4" sx={{ fontWeight: 'bold' }}>{formatCurrency(totalPending)}</Typography>
                                </Box>
                            </Paper>
                        </Grid>
                    </Grid>

                    <Divider sx={{ mb: 4 }} />
                    <Typography variant="h6" sx={{ mb: 3, fontWeight: 'bold', color: theme.palette.text.primary }}>
                        Histórico de Lançamentos
                    </Typography>

                    {commissions.length === 0 ? (
                        <Typography color="textSecondary">Nenhuma comissão registrada ainda.</Typography>
                    ) : (
                        <Grid container spacing={2}>
                            {commissions.map((item) => {
                                const statusConfig = getStatusConfig(item.status);
                                const formatedDate = new Date(item.dueDate).toLocaleDateString('pt-BR', { timeZone: 'UTC' });

                                return (
                                    <Grid size={{ xs: 12 }} key={item.id}>
                                        <Paper
                                            elevation={0}
                                            sx={{
                                                p: 3,
                                                borderRadius: 2,
                                                border: `1px solid ${theme.palette.divider}`,
                                                display: 'flex',
                                                flexDirection: { xs: 'column', md: 'row' },
                                                justifyContent: 'space-between',
                                                alignItems: { xs: 'flex-start', md: 'center' },
                                                gap: 2,
                                                transition: 'box-shadow 0.2s',
                                                '&:hover': { boxShadow: 1 }
                                            }}
                                        >
                                            <Box sx={{ flex: 1 }}>
                                                <Typography variant="h6" sx={{ fontWeight: 'bold', color: theme.palette.text.primary, fontSize: '1.1rem' }}>
                                                    {item.studentName}
                                                </Typography>
                                                <Typography variant="body2" color="text.secondary">
                                                    Contrato #{item.contractId} • Vencimento: {formatedDate}
                                                </Typography>
                                            </Box>

                                            <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: { xs: 'flex-start', md: 'flex-end' }, minWidth: '150px' }}>
                                                <Typography variant="body2" color="text.secondary">
                                                    Valor:
                                                </Typography>
                                                <Typography variant="h6" sx={{ fontWeight: 'bold', color: theme.palette.primary.main }}>
                                                    {formatCurrency(item.commissionAmount)}
                                                </Typography>
                                            </Box>

                                            <Chip
                                                icon={statusConfig.icon}
                                                label={item.status}
                                                color={statusConfig.color}
                                                variant={item.status === 'Recebido' ? 'filled' : 'outlined'}
                                                sx={{ fontWeight: 'bold', minWidth: '110px' }}
                                            />
                                        </Paper>
                                    </Grid>
                                );
                            })}
                        </Grid>
                    )}
                </>
            )}
        </Box>
    );


}