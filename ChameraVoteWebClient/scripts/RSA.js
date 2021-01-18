class RSA
{
    static n = 391;
    static PublicKey = [5,RSA.n];
    static ModInverse = function(a, b) {
        a %= b;
        for (var x = 1; x < b; x++) {
            if ((a*x)%b == 1) {
                return x;
            }
        }
    }
}

