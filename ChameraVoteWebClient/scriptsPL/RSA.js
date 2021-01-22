class RSA
{
    static n = 35;
    static PublicKey = [7,RSA.n];
    static ModInverse = function(a, b) {
        a %= b;
        for (var x = 1; x < b; x++) {
            if ((a*x)%b == 1) {
                return x;
            }
        }
    }
}

