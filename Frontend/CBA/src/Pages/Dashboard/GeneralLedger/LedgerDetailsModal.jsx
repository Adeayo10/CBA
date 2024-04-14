import Grid from "@mui/material/Grid";
import Button from "@mui/material/Button";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import DialogTitle from "@mui/material/DialogTitle";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import Divider from "@mui/material/Divider";

import { LEDGER_ALLLOWED_FIELDS } from "../../../utils/constants";

import { capitalize, formatCurrency, formatDate } from "../../../utils/util";

export default function LedgerDetailsModal({ toggleModal, modalOpen, ledger }) {
  if (!ledger.accountName) return <></>;

  return (
    <Dialog
      open={modalOpen}
      onClose={toggleModal}
      aria-labelledby="alert-dialog-title"
      aria-describedby="alert-dialog-description"
    >
      <DialogTitle id="alert-dialog-title">View Ledger</DialogTitle>

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
            {LEDGER_ALLLOWED_FIELDS.map((field, index) => {
              const fieldName = field.split(" ").join("");
              const fieldKey = `${ledger.id}_${fieldName}_${index}`;
              let fieldValue = capitalize(String(ledger[fieldName]));
              if (fieldName == "dateCreated")
                fieldValue = formatDate(fieldValue);
              if (fieldName == "balance") fieldValue = formatCurrency(fieldValue)

              return (
                <Grid xs={6} key={fieldKey} item>
                  <Typography
                    gutterBottom
                    variant="subtitle2"
                    color="text.secondary"
                  >
                    {capitalize(field)}
                  </Typography>
                  <Typography variant="body" gutterBottom>
                    {fieldValue}
                  </Typography>
                </Grid>
              );
            })}
          </Grid>
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={toggleModal} autoFocus>
          Done
        </Button>
      </DialogActions>
    </Dialog>
  );
}
