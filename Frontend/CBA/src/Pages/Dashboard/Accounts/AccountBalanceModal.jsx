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
import { TOAST_CONFIG, ACCOUNT_BALANCE_FIELDS } from "../../../utils/constants";
import { getCustomerBalance } from "../../../api/customer";

export default function AccountBalaceModal({
  toggleModal,
  modalOpen,
  account,
}) {
  if (!account.id) return <></>;

  const [accountBalance, setAccountBalance] = useState({
    ...ACCOUNT_BALANCE_FIELDS,
  });
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    if (modalOpen) {
      setIsLoading(true);
      getCustomerBalance(account.id)
        .then((data) => {
          if (data.errors || !data.status)
            throw new Error(data.message || data.errors);

          toast.success(data.message, TOAST_CONFIG);
          setIsLoading(false);
          setAccountBalance(data.data);
        })
        .catch((error) => {
          toast.error(error.message, TOAST_CONFIG);
          setIsLoading(false);
        });
    }
  }, [modalOpen, account]);

  const resetModal = () => {
    setIsLoading(false);
    setAccountBalance({ ...ACCOUNT_BALANCE_FIELDS });
    toggleModal();
  };

  return (
    <Dialog
      open={modalOpen}
      onClose={resetModal}
      aria-labelledby="alert-dialog-title"
      aria-describedby="alert-dialog-description"
    >
      <DialogTitle id="alert-dialog-title">
        View Account Balance Balance
      </DialogTitle>

      <DialogContent>
        <Divider sx={{ mb: 1, width: "100%" }} />
        <Box sx={{ width: "100%" }}>
          <Typography gutterBottom variant="h6">
            {account.accountType} Account: {account.accountNumber}
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
                {"Available Balance"}
              </Typography>
              <Typography variant="body" gutterBottom>
                {formatCurrency(accountBalance.availableBalance)}
              </Typography>
            </Grid>
            <Grid xs={12} item>
              <Typography
                gutterBottom
                variant="subtitle2"
                color="text.secondary"
              >
                {"Withdrawable Balance"}
              </Typography>
              <Typography variant="body" gutterBottom>
                {formatCurrency(accountBalance.withdrawableBalance)}
              </Typography>
            </Grid>
            <Grid xs={12} item>
              <Typography
                gutterBottom
                variant="subtitle2"
                color="text.secondary"
              >
                {"Ledger Balance"}
              </Typography>
              <Typography variant="body" gutterBottom>
                {formatCurrency(accountBalance.ledgerBalance)}
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
