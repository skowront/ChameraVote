class RSA
{
    static n = 21;
    static PublicKey = [23,RSA.n];
    static ModInverse = function(a, b) {
        a %= b;
        for (var x = 1; x < b; x++) {
            if ((a*x)%b == 1) {
                return x;
            }
        }
    }
}

