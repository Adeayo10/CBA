import Grid from "@mui/material/Grid";
import Button from "@mui/material/Button";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import DialogTitle from "@mui/material/DialogTitle";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import Divider from "@mui/material/Divider";
import Backdrop from "@mui/material/Backdrop";
import CircularProgress from "@mui/material/CircularProgress";

import { capitalize, formatCurrency, formatDate } from "../../../utils/util";
import { useState, useEffect } from "react";
import { getLedgerBalance } from "../../../api/ledger";
import { toast } from "react-toastify";
import { TOAST_CONFIG } from "../../../utils/constants";

export default function LedgerBalaceModal({ toggleModal, modalOpen, ledger }) {
  if (!ledger.accountName) return <></>;

  const [ledgerBalance, setLedgerBalance] = useState(0);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    if (modalOpen) {
      setIsLoading(true);
      getLedgerBalance(ledger.accountNumber)
        .then((data) => {
          if (data.errors || !data.status)
            throw new Error(data.message || data.errors);

          toast.success(data.message, TOAST_CONFIG);
          setIsLoading(false);
          setLedgerBalance(data.data.balance);
        })
        .catch((error) => {
          toast.error(error.message, TOAST_CONFIG);
          setIsLoading(false);
        });
    }
  }, [modalOpen, ledger]);

  const resetModal = () => {
    setIsLoading(false);
    setLedgerBalance(0);
    toggleModal();
  };

  return (
    <Dialog
      open={modalOpen}
      onClose={resetModal}
      aria-labelledby="alert-dialog-title"
      aria-describedby="alert-dialog-description"
    >
      <DialogTitle id="alert-dialog-title">View Ledger Balance</DialogTitle>

      <DialogContent>
        <Divider sx={{ mb: 1, width: "100%" }} />
        <Box sx={{ width: "100%" }}>
          <Typography gutterBottom variant="h6">
            Ledger Account: {ledger.accountName}
          </Typography>
          <Grid
            container
            rowSpacing={1}
            columnSpacing={{ xs: 1, sm: 2, md: 3 }}
            sx={{ my: 0.5, pt: 0.5, pb: 1.5 }}
          >
            <Grid xs={12} item>
              <Typography
                gutterBottom
                variant="subtitle2"
                color="text.secondary"
              >
                {capitalize("Balance")}
              </Typography>
              <Typography variant="body" gutterBottom>
                {formatCurrency(ledgerBalance)}
              </Typography>
            </Grid>
          </Grid>
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={resetModal} autoFocus>
          Done
        </Button>
      </DialogActions>
      <Backdrop
        sx={{ color: "#fff", zIndex: (theme) => theme.zIndex.drawer + 1 }}
        open={isLoading}
      >
        <CircularProgress color="inherit" />
      </Backdrop>
    </Dialog>
  );
}
