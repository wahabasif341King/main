import { useState, useEffect } from 'react';
import {
  Box,
  Typography,
  Button,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Chip,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  MenuItem,
  IconButton,
  Alert,
  CircularProgress,
  Collapse,
} from '@mui/material';
import { Add, Delete, KeyboardArrowDown, KeyboardArrowUp, ArrowForward } from '@mui/icons-material';
import { motion } from 'framer-motion';
import DashboardLayout from '../layouts/DashboardLayout.jsx';
import { getStockTransfers, createStockTransfer, updateStockTransferStatus, getWarehouses } from '../api/inventoryApi.js';
import { getProducts } from '../api/catalogApi.js';

const STATUS_FLOW = ['Draft', 'Requested', 'Approved', 'InTransit', 'Received'];
const STATUS_COLORS = {
  Draft: 'default',
  Requested: 'info',
  Approved: 'warning',
  InTransit: 'secondary',
  Received: 'success',
};

function nextStatus(current) {
  const idx = STATUS_FLOW.indexOf(current);
  return idx >= 0 && idx < STATUS_FLOW.length - 1 ? STATUS_FLOW[idx + 1] : null;
}

function TransferRow({ transfer, onAdvance }) {
  const [expanded, setExpanded] = useState(false);
  const next = nextStatus(transfer.status);

  return (
    <>
      <TableRow hover>
        <TableCell>
          <IconButton size="small" onClick={() => setExpanded(!expanded)}>
            {expanded ? <KeyboardArrowUp /> : <KeyboardArrowDown />}
          </IconButton>
        </TableCell>
        <TableCell>{transfer.fromWarehouseName}</TableCell>
        <TableCell><ArrowForward fontSize="small" sx={{ verticalAlign: 'middle' }} /></TableCell>
        <TableCell>{transfer.toWarehouseName}</TableCell>
        <TableCell>
          <Chip label={transfer.status} size="small" color={STATUS_COLORS[transfer.status] || 'default'} />
        </TableCell>
        <TableCell>{new Date(transfer.createdAt).toLocaleDateString()}</TableCell>
        <TableCell align="right">
          {next && (
            <Button size="small" variant="outlined" onClick={() => onAdvance(transfer.stockTransferId, next)}>
              Mark as {next}
            </Button>
          )}
        </TableCell>
      </TableRow>
      <TableRow>
        <TableCell colSpan={7} sx={{ py: 0, borderBottom: expanded ? undefined : 'none' }}>
          <Collapse in={expanded} timeout="auto" unmountOnExit>
            <Box sx={{ py: 2, pl: 6 }}>
              <Typography variant="subtitle2" color="text.secondary" gutterBottom>Items</Typography>
              {transfer.items.map((item, idx) => (
                <Typography key={idx} variant="body2">
                  {item.productName} — Qty: {item.quantity}
                </Typography>
              ))}
            </Box>
          </Collapse>
        </TableCell>
      </TableRow>
    </>
  );
}

function StockTransfers() {
  const [transfers, setTransfers] = useState([]);
  const [warehouses, setWarehouses] = useState([]);
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const [open, setOpen] = useState(false);
  const [fromWarehouseId, setFromWarehouseId] = useState('');
  const [toWarehouseId, setToWarehouseId] = useState('');
  const [items, setItems] = useState([{ productId: '', quantity: '' }]);
  const [saving, setSaving] = useState(false);

  const loadAll = async () => {
    setLoading(true);
    try {
      const [transfersRes, whRes, prodRes] = await Promise.all([
        getStockTransfers(),
        getWarehouses(),
        getProducts(),
      ]);
      setTransfers(transfersRes.data);
      setWarehouses(whRes.data);
      setProducts(prodRes.data);
    } catch (err) {
      setError('Failed to load stock transfers.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadAll();
  }, []);

  const handleOpen = () => {
    setFromWarehouseId('');
    setToWarehouseId('');
    setItems([{ productId: '', quantity: '' }]);
    setError('');
    setOpen(true);
  };

  const handleItemChange = (index, field, value) => {
    const updated = [...items];
    updated[index][field] = value;
    setItems(updated);
  };

  const addItemRow = () => setItems([...items, { productId: '', quantity: '' }]);
  const removeItemRow = (index) => setItems(items.filter((_, i) => i !== index));

  const handleSubmit = async () => {
    if (!fromWarehouseId || !toWarehouseId) {
      setError('Both From and To warehouse are required.');
      return;
    }
    if (fromWarehouseId === toWarehouseId) {
      setError("From and To warehouse can't be the same.");
      return;
    }
    const validItems = items.filter((i) => i.productId && i.quantity);
    if (validItems.length === 0) {
      setError('Add at least one product with quantity.');
      return;
    }

    setSaving(true);
    setError('');
    try {
      await createStockTransfer({
        fromWarehouseId,
        toWarehouseId,
        items: validItems.map((i) => ({ productId: i.productId, quantity: parseInt(i.quantity) })),
      });
      setOpen(false);
      loadAll();
    } catch (err) {
      setError(err.response?.data?.message || err.response?.data || 'Failed to create transfer.');
    } finally {
      setSaving(false);
    }
  };

  const handleAdvance = async (id, newStatus) => {
    try {
      await updateStockTransferStatus(id, newStatus);
      loadAll();
    } catch (err) {
      setError(err.response?.data?.message || err.response?.data || 'Failed to update status.');
    }
  };

  return (
    <DashboardLayout>
      <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} transition={{ duration: 0.4 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
          <Box>
            <Typography variant="h4" fontWeight={700}>Stock Transfers</Typography>
            <Typography variant="body2" color="text.secondary">Move stock between warehouses</Typography>
          </Box>
          <Button variant="contained" startIcon={<Add />} onClick={handleOpen}>
            New Transfer
          </Button>
        </Box>

        {error && !open && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

        <Paper elevation={0} sx={{ borderRadius: 3, overflow: 'hidden' }}>
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell />
                  <TableCell>From</TableCell>
                  <TableCell />
                  <TableCell>To</TableCell>
                  <TableCell>Status</TableCell>
                  <TableCell>Created</TableCell>
                  <TableCell align="right">Action</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {loading ? (
                  <TableRow><TableCell colSpan={7} align="center" sx={{ py: 4 }}><CircularProgress size={28} /></TableCell></TableRow>
                ) : transfers.length === 0 ? (
                  <TableRow><TableCell colSpan={7} align="center" sx={{ py: 4 }}><Typography color="text.secondary">No transfers yet.</Typography></TableCell></TableRow>
                ) : (
                  transfers.map((t) => (
                    <TransferRow key={t.stockTransferId} transfer={t} onAdvance={handleAdvance} />
                  ))
                )}
              </TableBody>
            </Table>
          </TableContainer>
        </Paper>
      </motion.div>

      <Dialog open={open} onClose={() => setOpen(false)} fullWidth maxWidth="sm">
        <DialogTitle>New Stock Transfer</DialogTitle>
        <DialogContent>
          {error && <Alert severity="error" sx={{ mb: 2, mt: 1 }}>{error}</Alert>}

          <TextField
            fullWidth
            select
            label="From Warehouse"
            value={fromWarehouseId}
            onChange={(e) => setFromWarehouseId(e.target.value)}
            sx={{ mt: 1, mb: 3 }}
          >
            {warehouses.map((w) => (
              <MenuItem key={w.warehouseId} value={w.warehouseId}>{w.name}</MenuItem>
            ))}
          </TextField>

          <TextField
            fullWidth
            select
            label="To Warehouse"
            value={toWarehouseId}
            onChange={(e) => setToWarehouseId(e.target.value)}
            sx={{ mb: 3 }}
          >
            {warehouses.map((w) => (
              <MenuItem key={w.warehouseId} value={w.warehouseId}>{w.name}</MenuItem>
            ))}
          </TextField>

          <Typography variant="subtitle2" sx={{ mb: 1 }}>Items</Typography>
          {items.map((item, index) => (
            <Box key={index} sx={{ display: 'flex', gap: 1, mb: 2 }}>
              <TextField
                select
                fullWidth
                label="Product"
                value={item.productId}
                onChange={(e) => handleItemChange(index, 'productId', e.target.value)}
              >
                {products.map((p) => (
                  <MenuItem key={p.productId} value={p.productId}>{p.name}</MenuItem>
                ))}
              </TextField>
              <TextField
                label="Qty"
                type="number"
                sx={{ width: 100 }}
                value={item.quantity}
                onChange={(e) => handleItemChange(index, 'quantity', e.target.value)}
              />
              <IconButton color="error" onClick={() => removeItemRow(index)} disabled={items.length === 1}>
                <Delete fontSize="small" />
              </IconButton>
            </Box>
          ))}
          <Button size="small" onClick={addItemRow}>+ Add Another Product</Button>
        </DialogContent>
        <DialogActions sx={{ p: 3, pt: 1 }}>
          <Button onClick={() => setOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleSubmit} disabled={saving}>
            {saving ? <CircularProgress size={20} color="inherit" /> : 'Create Transfer'}
          </Button>
        </DialogActions>
      </Dialog>
    </DashboardLayout>
  );
}

export default StockTransfers;