import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Drawer,
  AppBar,
  Toolbar,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Typography,
  IconButton,
  Avatar,
  Menu,
  MenuItem,
  Divider,
} from '@mui/material';
import {
  Dashboard as DashboardIcon,
  Inventory2,
  ShoppingCart,
  People,
  LocalShipping,
  Warehouse,
  Logout,
  Menu as MenuIcon,
  Category,
  Sell,
  ReceiptLong,
  Storefront,
  SwapHoriz,
  PersonAdd,
} from '@mui/icons-material';
import useAuthStore from '../store/authStore.js';

const drawerWidth = 260;

const menuItems = [
  { text: 'Dashboard', icon: <DashboardIcon />, path: '/dashboard' },
  { text: 'Team', icon: <PersonAdd />, path: '/team' },
  { text: 'Products', icon: <Inventory2 />, path: '/products' },
  { text: 'Categories', icon: <Category />, path: '/categories' },
  { text: 'Brands', icon: <Sell />, path: '/brands' },
  { text: 'Taxes', icon: <ReceiptLong />, path: '/taxes' },
  { text: 'Warehouses', icon: <Warehouse />, path: '/warehouses' },
  { text: 'Inventory', icon: <Storefront />, path: '/inventory' },
  { text: 'Stock Transfers', icon: <SwapHoriz />, path: '/stock-transfers' },
];

function DashboardLayout({ children }) {
  const navigate = useNavigate();
  const user = useAuthStore((state) => state.user);
  const logout = useAuthStore((state) => state.logout);
  const [anchorEl, setAnchorEl] = useState(null);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh' }}>
      {/* Sidebar */}
      <Drawer
        variant="permanent"
        sx={{
          width: drawerWidth,
          flexShrink: 0,
          '& .MuiDrawer-paper': {
            width: drawerWidth,
            boxSizing: 'border-box',
            backgroundColor: 'rgba(17, 24, 39, 0.9)',
            backdropFilter: 'blur(20px)',
            borderRight: '1px solid rgba(255, 255, 255, 0.06)',
          },
        }}
      >
        <Toolbar sx={{ py: 3 }}>
          <Typography variant="h5" fontWeight={800} sx={{ background: 'linear-gradient(90deg, #6366F1, #EC4899)', backgroundClip: 'text', WebkitBackgroundClip: 'text', color: 'transparent' }}>
            InventorySaaS
          </Typography>
        </Toolbar>

        <List sx={{ px: 2 }}>
          {menuItems.map((item) => (
            <ListItemButton
              key={item.text}
              onClick={() => navigate(item.path)}
              sx={{
                borderRadius: 2,
                mb: 0.5,
                '&:hover': { backgroundColor: 'rgba(99, 102, 241, 0.1)' },
              }}
            >
              <ListItemIcon sx={{ color: 'primary.light', minWidth: 40 }}>{item.icon}</ListItemIcon>
              <ListItemText primary={item.text} />
            </ListItemButton>
          ))}
        </List>
      </Drawer>

      {/* Main Content Area */}
      <Box sx={{ flexGrow: 1, display: 'flex', flexDirection: 'column' }}>
        {/* Top Navbar */}
        <AppBar
          position="static"
          elevation={0}
          sx={{
            backgroundColor: 'rgba(17, 24, 39, 0.7)',
            backdropFilter: 'blur(20px)',
            borderBottom: '1px solid rgba(255, 255, 255, 0.06)',
          }}
        >
          <Toolbar sx={{ justifyContent: 'flex-end' }}>
            <IconButton onClick={(e) => setAnchorEl(e.currentTarget)}>
              <Avatar sx={{ bgcolor: 'primary.main', width: 38, height: 38 }}>
                {user?.fullName?.charAt(0)?.toUpperCase() || 'U'}
              </Avatar>
            </IconButton>

            <Menu anchorEl={anchorEl} open={Boolean(anchorEl)} onClose={() => setAnchorEl(null)}>
              <Box sx={{ px: 2, py: 1 }}>
                <Typography variant="subtitle2" fontWeight={600}>
                  {user?.fullName}
                </Typography>
                <Typography variant="caption" color="text.secondary">
                  {user?.roles?.[0]}
                </Typography>
              </Box>
              <Divider />
              <MenuItem onClick={handleLogout}>
                <ListItemIcon>
                  <Logout fontSize="small" />
                </ListItemIcon>
                Logout
              </MenuItem>
            </Menu>
          </Toolbar>
        </AppBar>

        {/* Page Content */}
        <Box sx={{ flexGrow: 1, p: 4, backgroundColor: 'background.default' }}>
          {children}
        </Box>
      </Box>
    </Box>
  );
}

export default DashboardLayout;