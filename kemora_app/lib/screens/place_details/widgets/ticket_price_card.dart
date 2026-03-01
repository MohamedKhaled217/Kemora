import 'package:flutter/material.dart';

class TicketPriceCard extends StatefulWidget {
  final double priceAdult;
  final double priceChild;

  const TicketPriceCard({
    super.key,
    required this.priceAdult,
    required this.priceChild,
  });

  @override
  State<TicketPriceCard> createState() => _TicketPriceCardState();
}

class _TicketPriceCardState extends State<TicketPriceCard> {
  int adultCount = 1;
  int childCount = 0;

  @override
  Widget build(BuildContext context) {
    final total =
        (adultCount * widget.priceAdult) + (childCount * widget.priceChild);

    return Card(
      elevation: 0,
      color: Colors.white,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(16),
        side: const BorderSide(color: Colors.black12),
      ),
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Container(
                  padding: const EdgeInsets.all(8),
                  decoration: BoxDecoration(
                    color: Colors.blue.shade50,
                    shape: BoxShape.circle,
                  ),
                  child: const Icon(
                    Icons.attach_money,
                    color: Colors.blue,
                    size: 20,
                  ),
                ),
                const SizedBox(width: 8),
                const Text(
                  "Ticket Price",
                  style: TextStyle(fontWeight: FontWeight.bold, fontSize: 16),
                ),
              ],
            ),
            const SizedBox(height: 16),
            _buildCounterRow(
              "Adults (${widget.priceAdult.toStringAsFixed(0)} EGP)",
              adultCount,
              (val) => setState(() => adultCount = val),
            ),
            const SizedBox(height: 8),
            _buildCounterRow(
              "Children (${widget.priceChild.toStringAsFixed(0)} EGP)",
              childCount,
              (val) => setState(() => childCount = val),
            ),
            const Divider(height: 32),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                const Text(
                  "Total",
                  style: TextStyle(fontWeight: FontWeight.bold, fontSize: 16),
                ),
                Text(
                  "${total.toStringAsFixed(0)} EGP",
                  style: const TextStyle(
                    fontWeight: FontWeight.bold,
                    fontSize: 18,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 8),
            const Row(
              children: [
                Icon(Icons.payment, size: 16, color: Colors.grey),
                SizedBox(width: 4),
                Text(
                  "Payment: Cash, Visa, Both",
                  style: TextStyle(fontSize: 12, color: Colors.grey),
                ),
              ],
            ),
            const SizedBox(height: 16),
            SizedBox(
              width: double.infinity,
              child: ElevatedButton(
                onPressed: () {},
                style: ElevatedButton.styleFrom(
                  backgroundColor: Colors.cyan,
                  foregroundColor: Colors.white,
                  padding: const EdgeInsets.symmetric(vertical: 12),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                ),
                child: const Text("Order Tickets"),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildCounterRow(String label, int count, Function(int) onChanged) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Text(label, style: const TextStyle(color: Colors.grey)),
        Row(
          children: [
            IconButton(
              onPressed: count > 0 ? () => onChanged(count - 1) : null,
              icon: const Icon(
                Icons.remove_circle,
                color: Colors.grey,
                size: 20,
              ), // Using simpler icons for standard look
              padding: EdgeInsets.zero,
              constraints: const BoxConstraints(),
            ),
            SizedBox(width: 12, child: Center(child: Text("$count"))),
            IconButton(
              onPressed: () => onChanged(count + 1),
              icon: const Icon(Icons.add_circle, color: Colors.grey, size: 20),
              padding: EdgeInsets.zero,
              constraints: const BoxConstraints(),
            ),
          ],
        ),
      ],
    );
  }
}
