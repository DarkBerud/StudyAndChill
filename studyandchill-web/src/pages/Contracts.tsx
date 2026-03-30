import { useState, useEffect } from 'react';
import { Box, Typography, Paper, useTheme, Chip, Button, CircularProgress } from '@mui/material';
import Grid from '@mui/material/Grid';
import GavelIcon from '@mui/icons-material/Gavel';
import PictureAsPdfIcon from '@mui/icons-material/PictureAsPdf';
import PersonIcon from '@mui/icons-material/Person';
import CalendarTodayIcon from '@mui/icons-material/CalendarToday';
import AutorenewIcon from '@mui/icons-material/Autorenew';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';

interface Contract {
    id: number;
    teacherName: string;
    startDate: string;
    endDate: string;
    status: number;
    monthlyAmount: number;
    makeUpQuota: number;
    contractPdfUrl?: string | null;
}

export default function Contracts() {
    const theme = useTheme();
    const navigate = useNavigate();

    const [contracts, setContracts] = useState<Contract[]>([]);
    const [loading, setLoading] = useState(true);

    const getStatusConfig = (status: number) => {
        switch (status) {
            case 0: return { label: 'Ativo', color: 'success' as const, borderColor: theme.palette.success.main };
            case 1: return { label: 'Encerrado', color: 'default' as const, borderColor: theme.palette.text.disabled };
            case 2: return { label: 'Cancelado', color: 'error' as const, borderColor: theme.palette.error.main };
            default: return { label: 'Desconhecido', color: 'default' as const, borderColor: theme.palette.divider };
        }
    };

    const formatCurrency = (value: number) => {
        return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);
    };

    useEffect(() => {
        const fetchContracts = async () => {
            try {
                const response = await api.get('/contracts/my-contracts');

                setContracts(response.data);

            } catch (error) {
                console.error('Erro ao buscar contratos:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchContracts();
    }, []);

    return (
        <Box>
            <Box sx={{ display: 'flex', gap: 3, alignItems: 'center', mb: 4 }}>
                <Typography variant="h4" sx={{ fontFamily: 'Bodoni Moda', color: theme.palette.primary.main, fontWeight: 'bold' }}>
                    Meus Contratos
                </Typography>
                <Button variant="outlined" onClick={() => navigate('/dashboard')} sx={{ borderRadius: 3 }}>
                    Voltar
                </Button>
            </Box>

            {loading ? (
                <Box sx={{ display: 'flex', justifyContent: 'center', mt: 10 }}>
                    <CircularProgress />
                </Box>
            ) : contracts.length === 0 ? (
                <Paper elevation={0} sx={{ p: 4, textAlign: 'center', bgcolor: 'transparent', border: `1px dashed ${theme.palette.divider}` }}>
                    <GavelIcon sx={{ fontSize: 48, color: theme.palette.text.disabled, mb: 2 }} />
                    <Typography variant="h6" color="textSecondary">Nenhum contrato encontrado.</Typography>
                </Paper>
            ) : (
                <Grid container spacing={3}>
                    {contracts.map((contract) => {
                        const statusConfig = getStatusConfig(contract.status);
                        const startDateFormatted = new Date(contract.startDate).toLocaleDateString('pt-BR', { timeZone: 'UTC' });
                        const endDateFormatted = new Date(contract.endDate).toLocaleDateString('pt-BR', { timeZone: 'UTC' });

                        return (
                            <Grid size={{ xs: 12, lg: 6 }} key={contract.id}>
                                <Paper
                                    elevation={0}
                                    sx={{
                                        p: 4,
                                        borderRadius: theme.shape.borderRadius,
                                        border: `1px solid ${theme.palette.divider}`,
                                        borderTop: `6px solid ${statusConfig.borderColor}`,
                                        display: 'flex',
                                        flexDirection: 'column',
                                        gap: 3,
                                        transition: 'box-shadow 0.2s',
                                        '&:hover': { boxShadow: 2 }
                                    }}
                                >
                                    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                                            <GavelIcon sx={{ color: theme.palette.primary.main }} />
                                            <Typography variant="h6" sx={{ fontWeight: 'bold', color: theme.palette.text.primary }}>
                                                Contrato #{contract.id}
                                            </Typography>
                                        </Box>
                                        <Chip
                                            label={statusConfig.label}
                                            size="small"
                                            color={statusConfig.color}
                                            sx={{ fontWeight: 'bold' }}
                                        />
                                    </Box>

                                    <Grid container spacing={2}>
                                        <Grid size={{ xs: 12, sm: 6 }}>
                                            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, color: theme.palette.text.secondary, mb: 1 }}>
                                                <PersonIcon fontSize="small" />
                                                <Typography variant="body2">Professor(a)</Typography>
                                            </Box>
                                            <Typography variant="body1" sx={{ fontWeight: '500' }}>{contract.teacherName}</Typography>
                                        </Grid>

                                        <Grid size={{ xs: 12, sm: 6 }}>
                                            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, color: theme.palette.text.secondary, mb: 1 }}>
                                                <AttachMoneyIcon fontSize="small" />
                                                <Typography variant="body2">Mensalidade</Typography>
                                            </Box>
                                            <Typography variant="body1" sx={{ fontWeight: '500' }}>{formatCurrency(contract.monthlyAmount)}</Typography>
                                        </Grid>

                                        <Grid size={{ xs: 12, sm: 6 }}>
                                            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, color: theme.palette.text.secondary, mb: 1 }}>
                                                <CalendarTodayIcon fontSize="small" />
                                                <Typography variant="body2">Vigência</Typography>
                                            </Box>
                                            <Typography variant="body1" sx={{ fontWeight: '500' }}>
                                                {startDateFormatted} até {endDateFormatted}
                                            </Typography>
                                        </Grid>

                                        <Grid size={{ xs: 12, sm: 6 }}>
                                            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, color: theme.palette.text.secondary, mb: 1 }}>
                                                <AutorenewIcon fontSize="small" />
                                                <Typography variant="body2">Reposições Disponíveis</Typography>
                                            </Box>
                                            <Typography variant="body1" sx={{ fontWeight: '500' }}>
                                                {contract.makeUpQuota} aula(s)
                                            </Typography>
                                        </Grid>
                                    </Grid>

                                    <Box sx={{ mt: 1, pt: 3, borderTop: `1px dashed ${theme.palette.divider}` }}>
                                        <Button
                                            variant={contract.contractPdfUrl ? "contained" : "outlined"}
                                            color="primary"
                                            fullWidth
                                            startIcon={<PictureAsPdfIcon />}
                                            disabled={!contract.contractPdfUrl}
                                            sx={{ borderRadius: 2, py: 1.5 }}
                                            onClick={async () => {
                                                if (!contract.contractPdfUrl) return;
                                                try {
                                                    const response = await api.get(contract.contractPdfUrl, { responseType: 'blob' });

                                                    const pdfBlob = new Blob([response.data], { type: 'application/pdf' });
                                                    const url = window.URL.createObjectURL(pdfBlob);

                                                    window.open(url, '_blank');
                                                } catch (error) {
                                                    alert('Erro ao abrir o contrato. Tente novamente.');
                                                    console.error(error);
                                                }
                                            }}
                                        >
                                            {contract.contractPdfUrl ? 'Visualizar Contrato (PDF)' : 'PDF Indisponível'}
                                        </Button>
                                    </Box>
                                </Paper>
                            </Grid>
                        );
                    })}
                </Grid>
            )}
        </Box>
    );
}

function AttachMoneyIcon(props: React.SVGProps<SVGSVGElement>) {
    return (
        <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg" {...props}>
            <path d="M11.8 10.9C10.5 10.6 9.8 10.4 9.8 9.5C9.8 8.6 10.7 8.2 11.8 8.2C13.2 8.2 13.8 8.9 13.9 9.8H15.8C15.7 8.1 14.5 6.8 12.8 6.4V4H10.8V6.4C9.1 6.8 8 8.1 8 9.6C8 11.6 9.8 12.3 11.5 12.7C13.1 13.1 13.7 13.5 13.7 14.4C13.7 15.3 12.8 15.8 11.8 15.8C10.3 15.8 9.5 14.9 9.4 13.8H7.5C7.6 15.8 9.1 17.2 10.8 17.6V20H12.8V17.6C14.6 17.2 15.7 15.8 15.7 14.3C15.7 12 13.6 11.3 11.8 10.9Z" fill="currentColor" />
        </svg>
    );
}