import { Box, Container, Grid, Typography, IconButton, Paper, useTheme } from "@mui/material";
import WhatsAppIcon from '@mui/icons-material/WhatsApp';
import InstagramIcon from '@mui/icons-material/Instagram';
import { AddCircleOutline } from "@mui/icons-material";
import { useNavigate } from "react-router-dom";
import AnimatedLogo from "../components/AnimatedLogo";

const Home = () => {
    const theme = useTheme();
    const navigate = useNavigate();

    return (
        <Container
            maxWidth='lg'
            sx={{
                py: 4,
                minHeight: '100vh',
                display: 'flex',
                alignItems: 'center',
                bgcolor: theme.palette.background.default
            }}
        >
            <Grid container spacing={4} sx={{width: '100%', alignItems: 'stretch'}}>
                
                <Grid size={{xs: 12, md: 4}} sx={{display: 'flex', flexDirection: 'column', gap: 4}}>
                  <Box sx={{ textAlign: 'center', display: 'flex', flexDirection: 'column', alignItems: 'center'}}>
                    <AnimatedLogo size={220} />
                    <Box sx={{mt: 2, display: 'flex', gap: 2 }}>
                        <IconButton size='large' sx={{ color: theme.palette.secondary.main }}>
                            <WhatsAppIcon fontSize='large' />
                        </IconButton>
                        <IconButton size='large' sx={{ color: theme.palette.primary.main}}>
                            <InstagramIcon fontSize="large"/>
                        </IconButton>
                    </Box>
                    </Box>

                    <Paper
                        elevation={0}
                        sx={{
                            bgcolor: theme.palette.custom.purple,
                            height: 250,
                            p: 4,
                            cursor: 'pointer',
                            transition: 'transform 0.2s',
                            '&:hover': {transform: 'scale(1.02)' },
                            display: 'flex',
                            flexDirection: 'column',
                            justifyContent: 'flex-end',
                            borderRadius: theme.shape.borderRadius,
                        }}                    
                    >
                        <AddCircleOutline sx={{ position: 'absolute', top: 20, right: 20, color: '#fff' }} />
                        <Typography variant="h4" sx={{ color: '#fff', lineHeight: 1.2}}>
                            Mais<br />Informações
                        </Typography>
                    </Paper>
                </Grid>

                <Grid size={{ xs: 12, md: 8 }} sx={{ display: 'flex'}}>
                        <Paper
                            elevation={0}
                            sx={{
                                bgcolor: theme.palette.custom.lightBlue,
                                width: '100%',
                                minHeight: { xs: 300, md: '100%'},
                                p: 6,
                                cursor: 'pointer',
                                transition: 'transform 0.2s',
                                '&:hover': { transform: 'scale(1.02)' },
                                display: 'flex',
                                flexDirection: 'column',
                                justifyContent: 'flex-end',
                                borderRadius: theme.shape.borderRadius,
                            }}
                            onClick={() => navigate('/login')}
                        >
                            <Typography
                                variant="h3"
                                sx={{
                                    fontFamily: 'Bodoni Moda',
                                    fontWeight: 700,
                                    color: theme.palette.primary.main
                                }}
                            >
                                Login
                            </Typography>
                        </Paper>
                </Grid>

            </Grid>
        </Container>
    );
};

export default Home;