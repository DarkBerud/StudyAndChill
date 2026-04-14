import { useState, useEffect, useCallback } from 'react';
import {
    Box, Typography, Paper, useTheme, Button, TextField, MenuItem,
    Table, TableBody, TableCell, TableContainer, TableHead, TableRow, IconButton,
    Dialog, DialogTitle, DialogContent, DialogActions, Alert
} from '@mui/material';
import Grid from '@mui/material/Grid';
import DeleteIcon from '@mui/icons-material/Delete';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import WarningAmberIcon from '@mui/icons-material/WarningAmber';
import api from '../services/api';
import { useNavigate } from 'react-router-dom';

const WEEK_DAYS = [
    'Domingo', 'Segunda-feira', 'Terça-feira', 'Quarta-feira', 'Quinta-feira', 'Sexta-feira', 'Sábado'
];

interface Availability {
    id: number;
    dayOfWeek: number;
    availableFrom: string;
    availableTo: string;
    type: number;
}

interface ConflictItem {
    classId: number;
    date: string;
    studentName: string;
}

export default function MyAvailability() {
    const theme = useTheme();
    const navigate = useNavigate();
    const [availabilities, setAvailabilities] = useState<Availability[]>([]);

    const [dayOfWeek, setDayOfWeek] = useState<number>(1);
    const [availableFrom, setAvailableFrom] = useState('08:00');
    const [availableTo, setAvailableTo] = useState('12:00');
    const [type, setType] = useState<number>(3);

    const [conflictModalOpen, setConflictModalOpen] = useState(false);
    const [conflictList, setConflictList] = useState<ConflictItem[]>([]);

    const [editModalOpen, setEditModalOpen] = useState(false);
    const [editId, setEditId] = useState<number | null>(null);
    const [editDayOfWeek, setEditDayOfWeek] = useState<number>(1);
    const [editAvailableFrom, setEditAvailableFrom] = useState('08:00');
    const [editAvailableTo, setEditAvailableTo] = useState('12:00');
    const [editType, setEditType] = useState<number>(3);

    const timeSlots: string[] = [];
    for (let h = 7; h <= 23; h++) {
        for (let m = 0; m < 60; m += 15) {
            timeSlots.push(`${h.toString().padStart(2, '0')}:${m.toString().padStart(2, '0')}`);
        }
    }

    const fetchAvailabilities = useCallback(async () => {
        try {
            const response = await api.get('/teacheravailability');
            setAvailabilities(response.data);
        } catch (error) {
            console.error('Erro ao buscar horários', error);
        }
    }, []);

    useEffect(() => {
        const loadAvailabilities = async () => {
            await fetchAvailabilities();
        };

        void loadAvailabilities();
    }, [fetchAvailabilities]);


    const handleAdd = async () => {
        try {
            const payload = [{
                dayOfWeek,
                availableFrom: `${availableFrom}:00`,
                availableTo: `${availableTo}:00`,
                type
            }];
            await api.post('/teacheravailability', payload);
            alert('Horário adicionado com sucesso!');
            fetchAvailabilities();
        } catch (error: unknown) {
            const err = error as { response?: { data?: string } };
            alert(err.response?.data || 'Erro ao adicionar horário.');
        }
    };

    const handleDelete = async (id: number) => {
        if (!window.confirm('Tem certeza que deseja remover este horário da sua grade padrão?')) return;

        try {
            const response = await api.delete(`/teacheravailability/${id}`);

            if (response.data.hasConflicts) {
                setConflictList(response.data.conflicts);
                setConflictModalOpen(true);
            }

            fetchAvailabilities();
        } catch (error) {
            console.error('Erro ao remover', error);
            alert('Erro ao remover horário.');
        }
    };

    const handleEditClick = (av: Availability) => {
        setEditId(av.id);
        setEditDayOfWeek(av.dayOfWeek);
        setEditAvailableFrom(av.availableFrom.substring(0, 5));
        setEditAvailableTo(av.availableTo.substring(0, 5));
        setEditType(av.type);
        setEditModalOpen(true);
    };

    const handleUpdate = async () => {
        if (!editId) return;
        try {
            const payload = {
                dayOfWeek: editDayOfWeek,
                availableFrom: `${editAvailableFrom}:00`,
                availableTo: `${editAvailableTo}:00`,
                type: editType
            };
            await api.put(`/teacheravailability/${editId}`, payload);
            alert('Horário atualizado com sucesso!');
            setEditModalOpen(false);
            fetchAvailabilities();
        } catch (error: unknown) {
            const err = error as { response?: { data?: string } };
            alert(err.response?.data || 'Erro ao atualizar horário.');
        }
    };

    const getTypeLabel = (t: number) => {
        if (t === 1) return 'Aulas Regulares';
        if (t === 2) return 'Apenas Reposições';
        return 'Regulares e Reposições';
    };

    return (
        <Box>
            <Box sx={{ display: 'flex', gap: 3, alignItems: 'center', mb: 4 }}>
                <Typography variant="h4" sx={{ fontFamily: 'Bodoni Moda', color: theme.palette.primary.main, fontWeight: 'bold' }}>
                    Minha Grade de Horários
                </Typography>
                <Button variant="outlined" onClick={() => navigate('/dashboard')} sx={{ borderRadius: 3 }}>
                    Voltar
                </Button>
            </Box>

            <Paper elevation={0} sx={{ p: 3, mb: 4, borderRadius: 2, border: `1px solid ${theme.palette.divider}` }}>
                <Typography variant="h6" sx={{ mb: 3 }}>Adicionar Novo Horário</Typography>
                <Grid container spacing={2} alignItems="center">
                    <Grid size={{ xs: 12, sm: 3 }}>
                        <TextField select label="Dia da Semana" fullWidth value={dayOfWeek} onChange={(e) => setDayOfWeek(Number(e.target.value))}>
                            {WEEK_DAYS.map((dia, index) => (
                                <MenuItem key={index} value={index}>{dia}</MenuItem>
                            ))}
                        </TextField>
                    </Grid>

                    <Grid size={{ xs: 12, sm: 2 }}>
                        <TextField select label="Das (Início)" fullWidth value={availableFrom} onChange={(e) => setAvailableFrom(e.target.value)}>
                            {timeSlots.map(t => <MenuItem key={t} value={t}>{t}</MenuItem>)}
                        </TextField>
                    </Grid>

                    <Grid size={{ xs: 12, sm: 2 }}>
                        <TextField select label="Até (Fim)" fullWidth value={availableTo} onChange={(e) => setAvailableTo(e.target.value)}>
                            {timeSlots.map(t => <MenuItem key={t} value={t}>{t}</MenuItem>)}
                        </TextField>
                    </Grid>

                    <Grid size={{ xs: 12, sm: 3 }}>
                        <TextField select label="Tipo de Atendimento" fullWidth value={type} onChange={(e) => setType(Number(e.target.value))}>
                            <MenuItem value={1}>Aulas Regulares</MenuItem>
                            <MenuItem value={2}>Apenas Reposições</MenuItem>
                            <MenuItem value={3}>Ambos</MenuItem>
                        </TextField>
                    </Grid>

                    <Grid size={{ xs: 12, sm: 2 }}>
                        <Button variant="contained" fullWidth size="large" startIcon={<AddIcon />} onClick={handleAdd}>
                            Adicionar
                        </Button>
                    </Grid>
                </Grid>
            </Paper>

            <TableContainer component={Paper} elevation={0} sx={{ border: `1px solid ${theme.palette.divider}` }}>
                <Table>
                    <TableHead sx={{ bgcolor: theme.palette.action.hover }}>
                        <TableRow>
                            <TableCell><strong>Dia da Semana</strong></TableCell>
                            <TableCell><strong>Horário</strong></TableCell>
                            <TableCell><strong>Atendimento</strong></TableCell>
                            <TableCell align="right"><strong>Ações</strong></TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {availabilities.length === 0 ? (
                            <TableRow>
                                <TableCell colSpan={4} align="center" sx={{ py: 3, color: 'text.secondary' }}>
                                    Nenhum horário cadastrado. Comece a montar sua grade acima!
                                </TableCell>
                            </TableRow>
                        ) : (
                            availabilities.map((av) => (
                                <TableRow key={av.id}>
                                    <TableCell>{WEEK_DAYS[av.dayOfWeek]}</TableCell>
                                    <TableCell>{av.availableFrom.substring(0, 5)} às {av.availableTo.substring(0, 5)}</TableCell>
                                    <TableCell>{getTypeLabel(av.type)}</TableCell>
                                    <TableCell align="right">
                                        <IconButton color="primary" onClick={() => handleEditClick(av)}>
                                            <EditIcon />
                                        </IconButton>
                                        <IconButton color="error" onClick={() => handleDelete(av.id)}>
                                            <DeleteIcon />
                                        </IconButton>
                                    </TableCell>
                                </TableRow>
                            ))
                        )}
                    </TableBody>
                </Table>
            </TableContainer>

            <Dialog open={editModalOpen} onClose={() => setEditModalOpen(false)} maxWidth="sm" fullWidth>
                <DialogTitle sx={{ fontFamily: 'Bodoni Moda', fontWeight: 'bold' }}>Editar Horário</DialogTitle>
                <DialogContent sx={{ pt: 3 }}>
                    <Grid container spacing={2} sx={{ mt: 1 }}>
                        <Grid size={{ xs: 12, sm: 6 }}>
                            <TextField select label="Dia da Semana" fullWidth value={editDayOfWeek} onChange={(e) => setEditDayOfWeek(Number(e.target.value))}>
                                {WEEK_DAYS.map((dia, index) => (
                                    <MenuItem key={index} value={index}>{dia}</MenuItem>
                                ))}
                            </TextField>
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6 }}>
                            <TextField select label="Tipo de Atendimento" fullWidth value={editType} onChange={(e) => setEditType(Number(e.target.value))}>
                                <MenuItem value={1}>Aulas Regulares</MenuItem>
                                <MenuItem value={2}>Apenas Reposições</MenuItem>
                                <MenuItem value={3}>Ambos</MenuItem>
                            </TextField>
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6 }}>
                            <TextField select label="Das (Início)" fullWidth value={editAvailableFrom} onChange={(e) => setEditAvailableFrom(e.target.value)}>
                                {timeSlots.map(t => <MenuItem key={t} value={t}>{t}</MenuItem>)}
                            </TextField>
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6 }}>
                            <TextField select label="Até (Fim)" fullWidth value={editAvailableTo} onChange={(e) => setEditAvailableTo(e.target.value)}>
                                {timeSlots.map(t => <MenuItem key={t} value={t}>{t}</MenuItem>)}
                            </TextField>
                        </Grid>
                    </Grid>
                </DialogContent>
                <DialogActions sx={{ p: 2 }}>
                    <Button onClick={() => setEditModalOpen(false)} color="inherit">Cancelar</Button>
                    <Button onClick={handleUpdate} variant="contained" color="primary">Salvar Alterações</Button>
                </DialogActions>
            </Dialog>

            <Dialog open={conflictModalOpen} onClose={() => setConflictModalOpen(false)} maxWidth="sm" fullWidth>
                <DialogTitle sx={{ color: 'warning.main', display: 'flex', alignItems: 'center', gap: 1 }}>
                    <WarningAmberIcon /> Atenção: Aulas Afetadas
                </DialogTitle>
                <DialogContent>
                    <Alert severity="warning" sx={{ mb: 2 }}>
                        A sua disponibilidade padrão foi removida para o futuro. <strong>Porém</strong>, você ainda possui aulas agendadas neste dia/horário no mês atual que exigem sua atenção. Lembre-se de reagendá-las ou avisar os alunos!
                    </Alert>
                    <Typography variant="subtitle2" sx={{ mb: 1 }}>Aulas Pendentes neste horário:</Typography>
                    <ul>
                        {conflictList.map(c => (
                            <li key={c.classId}>
                                <strong>{c.date}</strong> - Aluno: {c.studentName}
                            </li>
                        ))}
                    </ul>
                </DialogContent>
                <DialogActions sx={{ p: 2 }}>
                    <Button variant="contained" onClick={() => setConflictModalOpen(false)}>Ciente, vou reagendar</Button>
                </DialogActions>
            </Dialog>
        </Box>
    );
}