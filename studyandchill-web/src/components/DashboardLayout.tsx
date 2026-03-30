import { Box, AppBar, Toolbar, Button, useTheme, Typography, Avatar } from "@mui/material";
import { Logout } from "@mui/icons-material";
import type React from "react";
import { useNavigate } from "react-router-dom";
import AnimatedLogo from "./AnimatedLogo";

export default function DashboardLayout({children}: {children: React.ReactNode}) {
    const theme = useTheme();
    const navigate = useNavigate();

    const userStr = localStorage.getItem('user');
    const user = userStr ? JSON.parse(userStr) : null;
    const initial = user?.name ? user.name.charAt(0).toUpperCase() : 'U';

    const handleLogout = () => {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        navigate('/login');
    };

    return (
        <Box sx={{display: 'flex', minHeight: '100vh', bgcolor: theme.palette.background.default}}>

            <AppBar
                position="fixed"
                elevation={0}
                sx={{
                    bgcolor: '#fff',
                    borderBottom: '1px solid ${theme.palette.divider}',
                    zIndex: theme.zIndex.drawer + 1
                }}
            >
                <Toolbar sx={{ justifyContent: 'space-between'}}>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 2}}>
                        <AnimatedLogo size={40} />
                        <Typography variant="h6" onClick={() => navigate('/dashboard')} sx={{ fontFamily: 'Bodoni Moda', color: theme.palette.primary.main, fontWeight: 'bold', cursor: 'pointer', transition: 'opacity 0.2s', '&:hover': {opacity: 0.8}}}>
                            Study & Chill
                        </Typography>
                    </Box>

                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 3}}>
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1}}>
                            <Avatar sx={{bgcolor: theme.palette.secondary.main, width: 32, height: 32}}>
                                {initial}
                            </Avatar>
                            <Typography sx={{ color: theme.palette.text.primary, fontWeight: 500, display: {xs: 'none', sm: 'block'}}}>
                                Welcome, {user?.name || 'Usuário'}
                            </Typography>
                        </Box>

                        <Button
                        onClick={handleLogout}
                        endIcon={<Logout/>}
                        sx={{ color: theme.palette.text.primary, fontWeight: 'bold'}}
                        >
                            Sair
                        </Button>
                    </Box>
                </Toolbar>
            </AppBar>
                <Box component='main' sx={{ flexGrow: 1, p: 4, pt: 12}}>
                    {children}
                </Box>
        </Box>
    )
}

